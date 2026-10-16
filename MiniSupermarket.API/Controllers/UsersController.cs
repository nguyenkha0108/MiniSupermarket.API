
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<UserRecord> _hasher = new();

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString =>
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Thiếu DefaultConnection.");

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword)
        {
            var users = new List<object>();

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                @"SELECT UserId, Username, Role, IsActive, CreatedAt
                  FROM dbo.Users
                  WHERE (@Keyword IS NULL
                     OR Username LIKE @Pattern
                     OR Role LIKE @Pattern)
                  ORDER BY UserId DESC", connection);

            var search = string.IsNullOrWhiteSpace(keyword)
                ? null : keyword.Trim();

            command.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100)
                .Value = (object?)search ?? DBNull.Value;
            command.Parameters.Add("@Pattern", SqlDbType.NVarChar, 210)
                .Value = "%" + (search ?? "") + "%";

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Role = reader.GetString(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                request.Username.Trim().Length > 50 ||
                string.IsNullOrWhiteSpace(request.Password) ||
                request.Password.Length < 8 ||
                !IsValidRole(request.Role))
            {
                return BadRequest(new
                {
                    message = "Tên đăng nhập tối đa 50 ký tự, mật khẩu ít nhất 8 ký tự và vai trò phải hợp lệ."
                });
            }

            var user = new UserRecord
            {
                Username = request.Username.Trim(),
                Role = request.Role,
                IsActive = true
            };

            var hash = _hasher.HashPassword(user, request.Password);

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                @"INSERT INTO dbo.Users (Username, PasswordHash, Role, IsActive)
                  VALUES (@Username, @PasswordHash, @Role, 1);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

            command.Parameters.Add("@Username", SqlDbType.NVarChar, 50)
                .Value = user.Username;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255)
                .Value = hash;
            command.Parameters.Add("@Role", SqlDbType.NVarChar, 20)
                .Value = user.Role;

            try
            {
                var id = (int)(await command.ExecuteScalarAsync())!;
                return CreatedAtAction(nameof(GetAll), new { id }, new
                {
                    UserId = id,
                    user.Username,
                    user.Role,
                    user.IsActive
                });
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                return Conflict(new { message = "Tên đăng nhập đã tồn tại." });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
        {
            if (!IsValidRole(request.Role))
                return BadRequest(new { message = "Vai trò không hợp lệ." });

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                @"UPDATE dbo.Users SET Role = @Role WHERE UserId = @Id;
                  SELECT @@ROWCOUNT;", connection);

            command.Parameters.Add("@Role", SqlDbType.NVarChar, 20)
                .Value = request.Role;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            var affected = (int)(await command.ExecuteScalarAsync())!;

            if (affected == 0)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            return Ok(new { message = "Cập nhật vai trò thành công." });
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> SetStatus(
            int id, [FromBody] UserStatusRequest request)
        {
            if (!request.IsActive &&
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    == id.ToString())
            {
                return BadRequest(new
                {
                    message = "Bạn không thể tự khóa tài khoản đang đăng nhập."
                });
            }

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                @"UPDATE dbo.Users SET IsActive = @IsActive WHERE UserId = @Id;
                  SELECT @@ROWCOUNT;", connection);

            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = request.IsActive;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            var affected = (int)(await command.ExecuteScalarAsync())!;

            if (affected == 0)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            return Ok(new
            {
                message = request.IsActive
                    ? "Đã mở khóa tài khoản."
                    : "Đã khóa tài khoản."
            });
        }

        [HttpPost("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int id, [FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) ||
                request.NewPassword.Length < 8)
            {
                return BadRequest(new
                {
                    message = "Mật khẩu mới phải có ít nhất 8 ký tự."
                });
            }

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var select = new SqlCommand(
                "SELECT UserId, Username, Role, IsActive FROM dbo.Users WHERE UserId=@Id",
                connection);
            select.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            UserRecord? user = null;

            await using (var reader = await select.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    user = new UserRecord
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Role = reader.GetString(2),
                        IsActive = reader.GetBoolean(3)
                    };
                }
            }

            if (user == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            var hash = _hasher.HashPassword(user, request.NewPassword);

            await using var update = new SqlCommand(
                "UPDATE dbo.Users SET PasswordHash=@Hash WHERE UserId=@Id",
                connection);
            update.Parameters.Add("@Hash", SqlDbType.NVarChar, 255).Value = hash;
            update.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await update.ExecuteNonQueryAsync();

            return Ok(new { message = "Đặt lại mật khẩu thành công." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                == id.ToString())
            {
                return BadRequest(new
                {
                    message = "Bạn không thể xóa tài khoản đang đăng nhập."
                });
            }

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "DELETE FROM dbo.Users WHERE UserId=@Id; SELECT @@ROWCOUNT;",
                connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            var affected = (int)(await command.ExecuteScalarAsync())!;

            if (affected == 0)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            return Ok(new { message = "Xóa tài khoản thành công." });
        }

        private static bool IsValidRole(string? role) =>
            role is "Admin" or "Cashier" or "Warehouse";
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Cashier";
    }

    public class UpdateUserRequest
    {
        public string Role { get; set; } = "";
    }

    public class UserStatusRequest
    {
        public bool IsActive { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = "";
    }
}

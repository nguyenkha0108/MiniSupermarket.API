
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<UserRecord> _hasher = new();

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString =>
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Chưa cấu hình ConnectionStrings:DefaultConnection");

        [HttpPost("setup-admin")]
        public async Task<IActionResult> SetupAdmin(
            [FromBody] SetupAdminRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                request.Password.Length < 8)
            {
                return BadRequest(new
                {
                    message = "Tên đăng nhập không được trống; mật khẩu phải có ít nhất 8 ký tự."
                });
            }

            await using var connection =
                new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var transaction =
                (SqlTransaction)await connection.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                await using var countCommand = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.Users WITH (TABLOCKX, HOLDLOCK)",
                    connection, transaction);

                var count = (int)(await countCommand.ExecuteScalarAsync() ?? 0);

                if (count > 0)
                {
                    await transaction.RollbackAsync();
                    return Conflict(new
                    {
                        message = "Hệ thống đã có tài khoản. Không thể khởi tạo Admin qua endpoint này."
                    });
                }

                var user = new UserRecord
                {
                    Username = request.Username.Trim(),
                    Role = "Admin",
                    IsActive = true
                };

                var hash = _hasher.HashPassword(user, request.Password);

                await using var insertCommand = new SqlCommand(
                    @"INSERT INTO dbo.Users
                      (Username, PasswordHash, Role, IsActive)
                      VALUES (@Username, @PasswordHash, 'Admin', 1)",
                    connection, transaction);

                insertCommand.Parameters.Add(
                    "@Username", SqlDbType.NVarChar, 50).Value = user.Username;
                insertCommand.Parameters.Add(
                    "@PasswordHash", SqlDbType.NVarChar, 255).Value = hash;

                await insertCommand.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Đã tạo tài khoản Admin đầu tiên. Hãy đăng nhập để tiếp tục."
                });
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                await transaction.RollbackAsync();
                return Conflict(new { message = "Tên đăng nhập đã tồn tại." });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Vui lòng nhập tài khoản và mật khẩu." });
            }

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                @"SELECT UserId, Username, PasswordHash, Role, IsActive
                  FROM dbo.Users
                  WHERE Username = @Username",
                connection);

            command.Parameters.Add(
                "@Username", SqlDbType.NVarChar, 50).Value =
                request.Username.Trim();

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu." });

            var user = new UserRecord
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3),
                IsActive = reader.GetBoolean(4)
            };

            if (!user.IsActive)
                return Unauthorized(new { message = "Tài khoản đã bị khóa." });

            var verification = _hasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password);

            if (verification == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu." });

            var secret = _configuration["JwtSettings:Secret"];

            if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
                return StatusCode(500, new
                {
                    message = "JwtSettings:Secret chưa được cấu hình an toàn."
                });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return Ok(new
            {
                success = true,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                username = user.Username,
                role = user.Role
            });
        }
    }

    public class UserRecord
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class SetupAdminRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}

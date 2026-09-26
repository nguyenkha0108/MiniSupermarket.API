using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string role = "";

            // ==========================
            // TÀI KHOẢN ADMIN
            // ==========================
            if (request.Username == "admin" &&
                request.Password == "123456")
            {
                role = "Admin";
            }

            // ==========================
            // TÀI KHOẢN CASHIER
            // ==========================
            else if (request.Username == "cashier" &&
                     request.Password == "123456")
            {
                role = "Cashier";
            }

            // ==========================
            // SAI TÀI KHOẢN
            // ==========================
            else
            {
                return Unauthorized(new
                {
                    message = "Sai tài khoản hoặc mật khẩu"
                });
            }

            // ==========================
            // JWT SECRET
            // ==========================
            var secretKey =
                _configuration["JwtSettings:Secret"];

            if (string.IsNullOrEmpty(secretKey))
            {
                return StatusCode(
                    500,
                    new
                    {
                        message =
                            "Chưa cấu hình JwtSettings:Secret"
                    });
            }

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            // ==========================
            // CLAIMS
            // ==========================
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    request.Username),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            // ==========================
            // TẠO TOKEN
            // ==========================
            var token =
                new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentials);

            var jwt =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);

            // ==========================
            // TRẢ VỀ WINFORMS
            // ==========================
            return Ok(new
            {
                success = true,
                token = jwt,
                role = role,
                username = request.Username
            });
        }
    }

    // DTO nhận dữ liệu từ WinForms
    public class LoginRequest
    {
        public string Username { get; set; }
            = string.Empty;

        public string Password { get; set; }
            = string.Empty;
    }
}
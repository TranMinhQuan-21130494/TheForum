using BackendAPI.Entities;
using BackendAPI.Exceptions;
using BackendAPI.Services;
using BackendAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendAPI.Controllers {
    [ApiController]
    [Route("api/account")]
    public class AccountController(IOptions<JwtSettings> jwtSettings, UserService userService) : ControllerBase {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly UserService _userService = userService;

        [HttpGet("ping")]
        public IActionResult Ping() {
            return Ok(new {
                Message = "OK"
            });
        }

        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GenerateToken(UserAuthenticationRequest request) {
            try {
                // Validate login credentials
                UserAuthenticationDTO userAuthentication = new() {
                    Email = request.Email,
                    RawPassword = request.Password,
                };
                UserDTO userDTO = _userService.GetOneUsingAuthentication(userAuthentication);

                // Create claims
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, userDTO.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Generate signing key
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Create token
                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                    signingCredentials: creds
                );

                // Return the token
                return Ok(new {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (EntityNotFoundException) {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (UnauthorizedException) {
                return Unauthorized(new { message = "Invalid username or password" });
            }
        }
    }
}

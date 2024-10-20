using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using AuthService.API.Models;
using AuthService.Application;

namespace AuthServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthServiceController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;

        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginModel model)
        {
            try
            {
                var token = await _authService.LoginAsync(model.Username, model.Password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials.");
            }
        }

        //// POST: api/auth/login
        //[HttpPost("login")]
        //public IActionResult Login([FromBody] LoginModel model)
        //{
        //    // Mock user validation
        //    if (model.Username != "testuser" || model.Password != "password")
        //    {
        //        return Unauthorized();
        //    }

        //    // Generate JWT
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[] {
        //            new Claim(ClaimTypes.Name, model.Username)
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        //        Issuer = _configuration["JwtSettings:Issuer"],
        //        Audience = _configuration["JwtSettings:Audience"]
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);

        //    return Ok(new { Token = tokenString });
        //}

        

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using AuthService.API.Models;
using AuthService.Application.Repository;

namespace AuthServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthServiceController : ControllerBase
    {
        #region private variable
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthServiceController> _logger;
        #endregion

        #region constructor 
        // Constructor for injecting dependencies (configuration and authentication service)
        public AuthServiceController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;

        }
        #endregion

        #region Login Authentication

        /// <summary>
        /// Authenticates the user and generates a JWT token upon successful login.
        /// </summary>
        /// <param name="model">The login model containing the username and password.</param>
        /// <returns>JWT token if authentication is successful; otherwise, returns Unauthorized status.</returns>

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginModel model)
        {
            // Validate the request model
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }

            try
            {
                // Authenticate the user via the AuthService
                var token = await _authService.LoginAsync(model.Username, model.Password);
                return Ok(new { Token = token });

            }
            catch (UnauthorizedAccessException)
            {
                // Return Unauthorized if credentials are invalid
                return Unauthorized("Invalid credentials.");                
            }
            catch (Exception ex)
            {
                // Log the exception and return an internal server error
                // (you can use a logger here if you have logging enabled)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        #endregion


    }
}

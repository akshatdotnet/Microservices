using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using AuthService.Infrastructure;


namespace AuthService.Application
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            // Mock user validation (Replace with real validation logic)
            var user = new User(username, "password"); // Mocked password
            if (!user.ValidatePassword(password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // Generate JWT token
            var token = await _tokenService.GenerateTokenAsync(user);
            return token;
        }
    }
}

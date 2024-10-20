using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure
{
   
        public interface ITokenService
        {
            Task<string> GenerateTokenAsync(User user);
        }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Repository
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
    }
}

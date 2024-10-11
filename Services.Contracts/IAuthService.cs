using Microsoft.AspNetCore.Identity;
using SharedAPI.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUser(RegisterRequest request);

        Task<bool> AuthenticateUser(LoginRequest loginRequest);

        Task<Tokens> GenerateToken(bool refreshExp);

        Task<Tokens> RefreshToken(Tokens tokens);
    }
}

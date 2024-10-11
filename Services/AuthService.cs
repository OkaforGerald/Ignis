using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using SharedAPI.DataTransfer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> manager;
        private readonly IConfiguration configManager;
        private readonly IMapper mapper;
        private IConfigurationSection JwtSettings;

        User? user;

        public AuthService(UserManager<User> manager, IMapper mapper, IConfiguration configManager)
        {
            this.configManager = configManager;
            this.manager = manager;
            this.mapper = mapper;

            JwtSettings = configManager.GetSection("JwtSettings");
        }

        public async Task<bool> AuthenticateUser(LoginRequest loginRequest)
        {
            this.user = await manager.FindByEmailAsync(loginRequest.Email);

            return this.user is not null && await manager.CheckPasswordAsync(this.user, loginRequest.Password);
        }

        public async Task<Tokens> GenerateToken(bool refreshExp)
        {
            var claims = GetClaims();
            var signingCreds = GetSigningCredentials();

            var token = new JwtSecurityToken(issuer: JwtSettings["Issuer"],
                audience: JwtSettings["Audience"], claims: claims,
                signingCredentials: signingCreds, expires: DateTime.Now.AddDays(Convert.ToDouble(JwtSettings["expires"])));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var bytes = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            user!.RefreshToken = Convert.ToBase64String(bytes);

            if (refreshExp) user.RefreshTokenExpiry = DateTime.Now.AddDays(7);

            await manager.UpdateAsync(user);

            return new Tokens { AccessToken =  jwtToken, RefreshToken = user.RefreshToken };
        }

        private SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings["SigningKey"])),
                SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email)

                //Maybe add more claims cause atp I no need am
            };
        }

        public async Task<IdentityResult> RegisterUser(RegisterRequest request)
        {
            var user = mapper.Map<User>(request);

            return await manager.CreateAsync(user, request.Password);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var parameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings["SigningKey"])),
                ValidIssuer = JwtSettings["Issuer"],
                ValidAudience = JwtSettings["Audience"],
                
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                
            };
            var principal = new JwtSecurityTokenHandler().ValidateToken(accessToken, parameters, out SecurityToken securityToken);
            var jwtToken = securityToken as JwtSecurityToken;

            if (jwtToken is null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase)) throw new SecurityTokenException("Invalid token!");

            return principal;
        }

        public async Task<Tokens> RefreshToken(Tokens tokens)
        {
            var principal = GetClaimsPrincipal(tokens.AccessToken);

            user = await manager.FindByEmailAsync(principal.Identity?.Name!);

            // Swap out for bad request later!
            if (user is null || !user.RefreshToken!.Equals(tokens.RefreshToken) || user.RefreshTokenExpiry < DateTime.Now) throw new Exception("Invalid refresh token!");

            return await GenerateToken(refreshExp: false);
        }
    }
}

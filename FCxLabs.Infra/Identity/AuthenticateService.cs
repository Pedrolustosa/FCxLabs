using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using FCxLabs.Domain.Account;
using Microsoft.Extensions.Configuration;
using FCxLabs.Domain.Entities;
using FCxLabs.Domain.Interfaces;

#nullable disable
namespace FCxLabs.Infra.Data.Identity
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IApplicationUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        public AuthenticateService(SignInManager<ApplicationUser> signInManager,
                                   IApplicationUserRepository userRepository,
                                   IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<bool> Authenticate(string userName, string password)
        {
            var statusUser = _userRepository.GetStatusUser(userName);
            if(statusUser)
            {
                var result = await _signInManager.PasswordSignInAsync(userName, password, false, lockoutOnFailure: false);
                return result.Succeeded;
            }
            throw new Exception("User Inactive!");
        }

        public string GenerateToken(string userName)
        {
            var claims = new[]
            {
                new Claim("email", userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);
            var expirations = DateTime.UtcNow.AddMinutes(7);

            JwtSecurityToken token = new
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expirations,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task Logout() => await _signInManager.SignOutAsync();
    }
}

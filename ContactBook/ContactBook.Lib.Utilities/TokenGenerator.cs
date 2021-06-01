 using ContactBook.Lib.Infrastructure;
using ContactBook.Lib.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Lib.Utilities
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtConfig _options;
        private readonly UserManager<AppUser> _userManager;
        public TokenGenerator(IOptionsMonitor<JwtConfig> options, UserManager<AppUser> userManager)
        {
            _options = options.CurrentValue;
            _userManager = userManager;
        }
        public async Task<string> GenerateToken(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_options.Secret);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }
                .Union(roleClaims)
                ),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}

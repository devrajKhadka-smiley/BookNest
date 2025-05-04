using BookNest.Data.Entities;
using BookNest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookNest.Services
{
    public class JwtServices
    {
        private readonly JwtTokenInfo _jwtTokenInfo;
        //Constructor DI
        public JwtServices(IOptions<JwtTokenInfo> jwtTokenInfoOption)
        {
            _jwtTokenInfo = jwtTokenInfoOption.Value;
        }

        public string GenerateToken(User user, IList<string> roles)
        {

            //Making Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            string key = _jwtTokenInfo.SecretKey;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenobj = new JwtSecurityToken(
                issuer: _jwtTokenInfo.Issuer,
                audience: _jwtTokenInfo.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtTokenInfo.ExpiryInMinute),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenobj);
        }
    }
}

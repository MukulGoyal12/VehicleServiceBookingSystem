using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OwnerService.Models;

namespace OwnerService.Helpers
{
    public class JWTHelper
    {
        // For demo purposes a hard-coded key is used. Move to configuration or secret store for production.

        public static string GenerateToken(Owner owner)

        {
            string SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET");
            string Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            string Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

            if (owner == null) throw new ArgumentNullException(nameof(owner));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("id", owner.OwnerId.ToString()),
                new Claim("name", owner.Name ?? string.Empty),
                new Claim("email", owner.Email ?? string.Empty),
                new Claim("phone", owner.Phone ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Sub, owner.OwnerId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

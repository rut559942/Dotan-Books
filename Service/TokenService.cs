using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Service
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            _key = jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing from configuration.");
            _issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing from configuration.");
            _audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing from configuration.");
        }

        public string GenerateToken(Customer customer)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new(ClaimTypes.Name, customer.Name),
                new(ClaimTypes.Email, customer.Email),
                new(ClaimTypes.Role, customer.IsAdmin ? "Admin" : "User"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
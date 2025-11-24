using LOAN_Web_API.Interfaces;
using LOAN_Web_API.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LOAN_Web_API.Services
{
    public class JwtGenerator : IJwtGenerator
    {
        // კონფიგურაცია უნდა წამოვიღოთ AppSettings-დან (Dependency Injection)
        private readonly IConfiguration _config;

        public JwtGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            // Claims - ტოკენში შესანახი ინფორმაცია (User ID, Role, UserName)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // 1. უსაფრთხოების გასაღები
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ??
                throw new InvalidOperationException("JWT Key is not configured.")));

            // 2. ხელმოწერის მონაცემები
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 3. ტოკენის აღწერა
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), // ვადა 7 დღე
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            // 4. ტოკენის შექმნა
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

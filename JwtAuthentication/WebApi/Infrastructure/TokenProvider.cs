using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebApi.Models;

namespace WebApi.Infrastructure
{
    public class TokenProvider
    {
        private readonly IConfiguration _configuration;
        public TokenProvider(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public Token GenerateToken(UserAccount userAccount)
        {
            var accessToken = GenerateAccessToken(userAccount);
            return new Token { AccessToken = (string)accessToken };
        }

        private string GenerateAccessToken(UserAccount userAccount)
        {
            string secretKey = _configuration["JWT:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                (
                    [
                        new Claim(ClaimTypes.Email, userAccount.Email),
                        new Claim(ClaimTypes.Role, userAccount.Role)
                    ]
                ),

                Expires = DateTime.Now.AddMinutes(1),
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}

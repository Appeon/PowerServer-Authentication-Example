using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerAPIs.Authentication.Common;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace ServerAPIs.Authentication.Jwt
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenResponse CreateToken(JwtUser user)
        {
            var response = new TokenResponse();

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiresTime = DateTime.UtcNow.AddSeconds(response.ExpireIn);

            var securityKey = _configuration["Jwt:SecurityKey"];
            var symmetricKey = new SymmetricSecurityKey(Convert.FromBase64String(securityKey));
            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);
            var payload = new JwtPayload(issuer, audience, user.Claims, null, expiresTime);

            response.AccessToken = _handler.WriteToken(new JwtSecurityToken(header, payload));

            return response;
        }
    }
}

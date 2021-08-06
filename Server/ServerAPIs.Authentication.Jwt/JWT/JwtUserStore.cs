using IdentityModel;
using Microsoft.Extensions.Logging;
using ServerAPIs.Authentication.Common;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.Jwt
{
    public class JwtUserStore : UserStoreBase<JwtUser>
    {
        public JwtUserStore(ILogger<JwtUserStore> logger)
        {
            base.logger = logger;

            users = new List<JwtUser>
            {
                new JwtUser
                {
                    Username = "alice",
                    Password = "alice",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Scope, "serverapi"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                    }
                },
                new JwtUser
                {
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Scope, "serverapi"),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                    }
                }
            };
        }

        public override Task<JwtUser> ValidateAsync(string username, string password)
        {
            var user = users.FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user != null)
            {
                logger.LogInformation($"User <{username}> logged in.");

                return Task.FromResult(user);
            }
            else
            {
                logger.LogError($"Invalid login attempt.");

                return default;
            }
        }
    }
}

using IdentityModel;
using IdentityServer4;
using Microsoft.Extensions.Logging;
using ServerAPIs.Authentication.Common;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.IdentityServer4
{
    public class IdSvr4UserStore : UserStoreBase<IdSvr4User>
    {
        public IdSvr4UserStore(ILogger<IdSvr4UserStore> logger)
        {
            base.logger = logger;

            var address = JsonSerializer.Serialize(new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            });

            users = new List<IdSvr4User>
            {
                new IdSvr4User
                {
                    SubjectId = "818727",
                    Username = "alice",
                    Password = "alice",
                    IsActive = true,
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Address, address, IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new IdSvr4User
                {
                    SubjectId = "88421113",
                    Username = "bob",
                    Password = "bob",
                    IsActive = true,
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.Address, address, IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
        }

        public override Task<IdSvr4User> ValidateAsync(string username, string password)
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

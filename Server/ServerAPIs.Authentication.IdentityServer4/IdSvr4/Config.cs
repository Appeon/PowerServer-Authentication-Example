using IdentityServer4.Models;
using System.Collections.Generic;

namespace ServerAPIs.Authentication.IdentityServer4
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("serverapi", "PowerServer Api"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedScopes = { "serverapi" },
                    AllowOfflineAccess = true,
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "Resource Owner Password Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets = { new Secret("08692CED-944D-4DA9-BFEF-0FE503C203AC".Sha256()) },
                    AllowedScopes = { "openid", "profile", "serverapi" },
                    AllowOfflineAccess = true,
                },
            };
    }
}
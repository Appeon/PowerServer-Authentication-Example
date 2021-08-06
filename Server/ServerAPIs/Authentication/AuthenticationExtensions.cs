using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PowerServer.Core;

namespace ServerAPIs
{
    public static class AuthenticationExtensions
    {
        // To be called in Startup.ConfigureServices, for adding services related with the authentication module
        public static IServiceCollection AddPowerServerAuthentication(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationPlatform();

            // The following five authentication services are registered（IdentityServer4、IdentityModel JWT、AWS Cognito、Azure AD、Azure B2C）
            services.AddIdSvr4(configuration);
            services.AddJwt(configuration);
            services.AddAWSCognito(configuration);
            services.AddAzureAD(configuration);
            services.AddAzureB2C(configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PowerServerConstants.DefaultAuthorizePolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            //The current template does not provide authentication service. If you want to use authentication service, change to another template
            return services;
        }
    }
}

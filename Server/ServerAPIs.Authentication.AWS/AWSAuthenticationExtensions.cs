using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using ServerAPIs.Authentication.AWS;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AWSAuthenticationExtensions
    {
        public static IServiceCollection AddAWSCognito(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<AWSUserStore>();
            services.AddSingleton<CognitoTokenManager>();

            services.AddAuthentication()
                .AddJwtBearer("AmazonCognito", options =>
                {
                    var region = configuration["AWS:Region"];
                    var poolID = configuration["AWS:UserPoolId"];

                    options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{poolID}";

                    options.TokenValidationParameters.ValidateAudience = false;

                    options.SaveToken = true;

                    options.IncludeErrorDetails = true;

                    IdentityModelEventSource.ShowPII = true;
                });

            services.AddTransient<IStartupFilter, AWSAuthenticationMiddleware>();

            return services;
        }
    }
}

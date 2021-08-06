using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using ServerAPIs.Authentication.Jwt;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwt(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Add the JWT services.
            services.AddSingleton<JwtUserStore>();
            services.AddSingleton<JwtHandler>();

            // Add the authentication of serverAPI.
            services.AddAuthentication()
                .AddJwtBearer("Jwt", options =>
                {
                    var issuer = configuration["Jwt:Issuer"];
                    var audience = configuration["Jwt:Audience"];

                    var securityKey = configuration["Jwt:SecurityKey"];
                    var symmetricKey = new SymmetricSecurityKey(Convert.FromBase64String(securityKey));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = symmetricKey,

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                    };

                    options.SaveToken = true;

                    options.IncludeErrorDetails = true;

                    IdentityModelEventSource.ShowPII = true;
                });

            services.AddTransient<IStartupFilter, JwtAuthenticationMiddleware>();

            return services;
        }
    }
}

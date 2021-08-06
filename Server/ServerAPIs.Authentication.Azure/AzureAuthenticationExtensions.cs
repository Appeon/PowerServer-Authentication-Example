using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using ServerAPIs.Authentication.Azure;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AzureAuthenticationExtensions
    {
        public static IServiceCollection AddAzureAD(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AzureADOptions>()
                        .Configure<IConfiguration>((opts, configuration) =>
                        {
                            configuration.Bind(AzureADOptions.Position, opts);
                        });

            services.AddAuthentication()
                .AddJwtBearer("AzureAD", options =>
                {
                    var authority = configuration["AzureAD:Authority"];

                    options.Authority = authority;

                    options.RequireHttpsMetadata =
                        !string.IsNullOrWhiteSpace(options.Authority) &&
                        options.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                    options.TokenValidationParameters.ValidateAudience = false;

                    options.SaveToken = true;

                    options.IncludeErrorDetails = true;

                    IdentityModelEventSource.ShowPII = true;
                });

            services.AddSingleton<AzureADTokenManager>();
            services.AddTransient<IStartupFilter, AzureADAuthenticationMiddleware>();

            return services;
        }

        public static IServiceCollection AddAzureB2C(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<AzureB2COptions>()
                        .Configure<IConfiguration>((opts, configuration) =>
                        {
                            configuration.Bind(AzureB2COptions.Position, opts);
                        });

            services.AddAuthentication()
                .AddJwtBearer("AzureB2C_Code", options =>
                {
                    var authority = configuration["AzureB2C:Authority_Code"];

                    options.Authority = authority;

                    options.RequireHttpsMetadata =
                        !string.IsNullOrWhiteSpace(options.Authority) &&
                        options.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                    options.TokenValidationParameters.ValidateAudience = false;

                    options.SaveToken = false;

                    options.IncludeErrorDetails = true;

                    IdentityModelEventSource.ShowPII = true;
                });

            services.AddAuthentication()
                .AddJwtBearer("AzureB2C_Pwd", options =>
                {
                    var authority = configuration["AzureB2C:Authority_Pwd"];

                    options.Authority = authority;

                    options.RequireHttpsMetadata =
                        !string.IsNullOrWhiteSpace(options.Authority) &&
                        options.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                    options.TokenValidationParameters.ValidateAudience = false;

                    options.SaveToken = true;

                    options.IncludeErrorDetails = true;

                    IdentityModelEventSource.ShowPII = true;
                });

            services.AddSingleton<AzureB2CTokenManager>();
            services.AddTransient<IStartupFilter, AzureB2CAuthenticationMiddleware>();

            return services;
        }
    }
}

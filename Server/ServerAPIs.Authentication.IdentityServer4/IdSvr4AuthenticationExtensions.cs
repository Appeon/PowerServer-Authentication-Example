using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using ServerAPIs.Authentication.IdentityServer4;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdSvr4AuthenticationExtensions
    {
        public static IServiceCollection AddIdSvr4(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<IdSvr4Options>()
                        .Configure<IConfiguration>((opts, configuration) =>
                        {
                            configuration.Bind(IdSvr4Options.Position, opts);
                        });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.EmitStaticAudienceClaim = true;
            });

            builder.AddInMemoryClients(Config.Clients);
            builder.AddInMemoryApiScopes(Config.ApiScopes);
            builder.AddInMemoryIdentityResources(Config.IdentityResources);

            builder.Services.AddSingleton<IdSvr4UserStore>();
            builder.AddResourceOwnerValidator<UserValidator>();

            // not recommended for production - you need to store your key material somewhere secure.
            builder.AddDeveloperSigningCredential();

            // Add the authentication of serverAPI.
            services.AddAuthentication()
                .AddJwtBearer("IdSvr4", options =>
                {
                    options.Authority = configuration["IdSvr4:Authority"];

                    options.RequireHttpsMetadata = !string.IsNullOrWhiteSpace(options.Authority) &&
                        options.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                    options.TokenValidationParameters.ValidateAudience = false;

                    options.SaveToken = true;

                    options.IncludeErrorDetails = true;

                    IdentityModelEventSource.ShowPII = true;
                });

            services.AddTransient<IStartupFilter, IdSvr4AuthenticationMiddleware>();

            return services;
        }
    }
}
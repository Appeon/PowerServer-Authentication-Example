using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using PowerServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerAPIs
{
    public static class HealthChecksExtensions
    {
        // To be called in Startup.ConfigureServices, for adding services related with the HealthCheck module
        public static IServiceCollection AddPowerServersHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            // Configures the rate of requests to the HealthCheck APIs, based on IpRateLimitMiddleware framework
            // see https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware

            // Loads general configuration from IpRateLimiting in HealthChecks.json
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            // Loads the IP rules from IpRateLimiting in HealthChecks.json
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // Stores the rate limit counters and IP rules
            services.AddMemoryCache();

            // Injects the counter and rules stores
            services.AddInMemoryRateLimiting();
            //services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
            //services.AddDistributedRateLimiting<RedisProcessingStrategy>();
            //services.AddRedisRateLimiting();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Injects the counter and rules distributed cache stores
            //services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            //services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            // Configures the HealthCheck publisher
            // see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks#health-check-publisher
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Period = TimeSpan.FromSeconds(30);
                options.Predicate = ctx => ctx.Tags.Intersect(PowerServerConstants.HealthCheckTags.All).Any();
            });

            // Uncomment the script below if you want to add the publisher for scheduled health check and outputs the result to the specified monitor system
            //services.AddSingleton<IHealthCheckPublisher, HealthChecksPublisher>();

            return services;
        }

        // To be called in Startup.ConfigureServices, for adding the HealthCheck module to the HTTP request pipeline
        public static IApplicationBuilder UsePowerServerHealthChecks(this IApplicationBuilder app)
        {
            // add IpRateLimitMiddleware to the HTTP request pipeline
            app.UseIpRateLimiting();

            var directory = Path.Combine(AppContext.BaseDirectory, "HealthChecks", "health-ui");

            // Adds the file server to access the HealthCheck output UI
            app.UseFileServer(new FileServerOptions
            {
                // Configures the path to the HealthCheck UI 
                RequestPath = "/health-ui",

                // Sets whether to use the default files, such as index.html and default.html
                // If true, no need to display the input file path when accessing the default files
                EnableDefaultFiles = true,

                // Sets the physical directory in which the file server provides the file services
                FileProvider = new PhysicalFileProvider(directory)
            });

            app.UseEndpoints(endpoints =>
            {
                // Sets the HealthCheck summary API. It checks all items and returns a summary result
                endpoints.MapHealthChecks("/health");

                // Sets the PowerServer liveness-checking API
                // Can be used by Kubernetes liveness probe. If the result is unhealthy, the service will be restarted
                endpoints.MapHealthChecks("/health-liveness", new HealthCheckOptions
                {
                    // Do not perform any checks. Only check whether the service is accessible
                    Predicate = x => false,
                });

                //  Sets the PowerServer readiness-checking API
                // Can be used by Kubernetes readiness probe. If the result is unhealthy, no further requests will be redirected
                endpoints.MapHealthChecks("/health-readiness", new HealthCheckOptions
                {
                    // Checks on all items that affect the handling of requests and see if the service can handle further requests
                    Predicate = ctx =>
                    {
                        var tags = new HashSet<string>
                        {
                            // Checks whether the license is in normal state
                            PowerServerConstants.HealthCheckTags.License,
                            // Checks whether the database connection is in normal state
                            PowerServerConstants.HealthCheckTags.Connection,
                        };

                        return ctx.Tags.Any(tag => tags.Contains(tag));
                    }
                });

                // Sets the PowerServer self-check API, to perform all the internal health check programs and return the details
                endpoints.MapHealthChecks($"/health-details", new HealthCheckOptions
                {
                    // Rewrite the response for UI display
                    ResponseWriter = HealthChecksResponseWriter.WriteUIResponse,

                    // Perform all the internal health check programs
                    Predicate = ctx => ctx.Tags.Intersect(PowerServerConstants.HealthCheckTags.All).Any(),

                    // Sets the corresponding status code of all checks to 200
                    ResultStatusCodes = new Dictionary<HealthStatus, int>
                    {
                        { HealthStatus.Healthy, StatusCodes.Status200OK },
                        { HealthStatus.Degraded, StatusCodes.Status200OK },
                        { HealthStatus.Unhealthy, StatusCodes.Status200OK },
                    }
                });
            });

            return app;
        }
    }
}

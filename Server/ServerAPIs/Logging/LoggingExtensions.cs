using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServerAPIs
{
    public static class LoggingExtensions
    {
        // Called by Program.CreateHostBuilder, for adding the PowerServer logging functionality
        public static IHostBuilder ConfigurePowerServerLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureLogging(builder =>
            {
                // Sets Log4Net as the logging program
                // see https://github.com/huorswords/Microsoft.Extensions.Logging.Log4Net.AspNetCore
                builder.AddLog4Net("Logging/log4net.xml", true);
            });

            return hostBuilder;
        }

        // To be called by Startup.ConfigureServices, for adding services from the PowerServer logging module
        public static IServiceCollection AddPowerServerLogging(this IServiceCollection services)
        {
            // Sets the directory browser
            services.AddDirectoryBrowser();

            // Uncomment the script below if you want to enable Azure Application Insights 
            // see https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
            // Note: Please add valid key for Azure Application Insights in Logging.json 
            //services.AddApplicationInsightsTelemetry();

            return services;
        }

        // To be called by Startup.ConfigureServices, for adding the requests from PowerServer logging module in the HTTP request pipeline
        public static IApplicationBuilder UsePowerServerLogging(this IApplicationBuilder application)
        {
            var configuration = application.ApplicationServices.GetRequiredService<IConfiguration>();

            // Loads configuration from Logging.json on whether to enable file server for logging
            if (configuration.GetValue<bool>("Logging:EnableFileServer"))
            {
                var options = new FileServerOptions();

                // Sets the request path of the logging file server
                options.RequestPath = "/logs";

                // Whether to enable directory browsing
                options.EnableDirectoryBrowsing = true;

                // Wehther to browse any file type
                options.StaticFileOptions.ServeUnknownFileTypes = true;

                // The physical directory in the logging file server
                var directory = Path.Combine(AppContext.BaseDirectory, "Logging", "logs");

                options.FileProvider = new PhysicalFileProvider(directory, ExclusionFilters.Sensitive);

                // Enables a file server to provide the file browsing service in the logging directory
                // Does not enable the service if the log may contain sensitive information
                application.UseFileServer(options);
            }

            return application;
        }
    }
}

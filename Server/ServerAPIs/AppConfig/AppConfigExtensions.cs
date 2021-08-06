using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServerAPIs
{
    public static class AppConfigExtensions
    {
        //To be called in Startup.ConfigureServices, for adding services related with the PowerServer configuration management module
        public static IServiceCollection AddPowerServerAppConfig(
            this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            //Reads the static application configuration and db configuration
            services.AddAppConfigFromFileSystem(context =>
            {
                // ASP.NET Core configuration, to get relevant configuration data (environment variables, commandline argument from different sources
                // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/
                context.Configuration = configuration;

                // The folder of the configuration file. It shall be the path relative to the project folder, and can also be the absolute path or shared path
                context.AppConfigDirectory = "AppConfig";

                // ASP.NET Core hosting environment, for reading the root directory of the application
                // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host#ihostenvironment
                context.HostingEnvironment = hostingEnvironment;

                //Uncomment the script below if you want to read configuration from self-defined program/file
                //context.ProviderFactory = (applicationsFilePath, argumentConfigurationProvider) =>
                //{
                //    // Self define the program/file that provides the configuration. It may be useful for some scenarios, such as, you need to refresh the db connection password periodically
                //    // It is recommended to inherit FileSystemConfigurationProvider and only customize some necessary logics
                //    return new YourCustomConfigurationProvider(applicationsFilePath, argumentConfigurationProvider);
                //};
            });

            return services;
        }
    }
}

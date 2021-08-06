using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ServerAPIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

#if DEBUG
            // Keeps the console open if the program stops. This is for debugging possible errors
            // Do not use this script in production environment, otherwise the program may not exit properly at exceptions
            Console.WriteLine("Press any key to close this window ...");
            Console.ReadKey();
#endif
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);

            // Sets the PowerServer logging provider
            hostBuilder.ConfigurePowerServerLogging();

            // Sets the PowerServer host
            // Handles the injection of configuration file, Web host, and PowerServer key services
            hostBuilder.ConfigurePowerServerHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

            return hostBuilder;
        }
    }
}

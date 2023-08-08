using PowerServer.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserExtensions;

namespace ServerAPIs
{
    /// <summary>
    /// Do NOT modify this file. Otherwise, unexpected upgrade issues may occur.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            this.Configuration = configuration;
            this.HostingEnvironment = hostingEnvironment;

            this.UserStartup = new UserStartup(this.Configuration, this.HostingEnvironment);
        }

        public UserStartup UserStartup { get; }

        // ASP.NET Core configuration, for getting the configuration data from different configuration sources (such as environment variables, commandline arguments)
        // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/
        public IConfiguration Configuration { get; }

        // ASP.NET Core hosting environment, for getting the root directory and startup environment of the application
        // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host#ihostenvironment
        public IHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adds the PowerServer core service
            services.AddPowerServerCore(this.Configuration, this.HostingEnvironment);

            //Configure user serivce
            this.UserStartup.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //Calls the PowerServer core service
            app.UsePowerServerCore(this.Configuration, this.HostingEnvironment);

            if (this.HostingEnvironment.IsDevelopment())
            {
                app.UsePowerServerFileServer();
            }

            //Calls the user service
            this.UserStartup.Configure(app);
        }
    }
}

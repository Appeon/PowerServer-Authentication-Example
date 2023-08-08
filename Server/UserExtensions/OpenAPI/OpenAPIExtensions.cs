using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace UserExtensions
{
    public static class OpenAPIExtensions
    {
        // To be called by startup.ConfigureServices,  for adding the PowerServer OpenAPI functionality 
        public static IServiceCollection AddPowerServerOpenAPI(this IServiceCollection services)
        {
            // Creates the Swagger generator
            // see https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle
            services.AddSwaggerGen(options =>
            {
                // Adds the description information for the API document
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    // API version
                    Version = "v1",

                    // API document title
                    Title = "PowerServer API",

                    // API document description
                    Description = "PowerServer management APIs",
                });

                var filePath = Path.Combine(AppContext.BaseDirectory, "OpenAPI", "Swagger.xml");

                // Imports the XML Comments document
                options.IncludeXmlComments(filePath, true);

                // Sets the authorization  to be used by OpenAPI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    // Authorization name
                    Name = "Authorization",

                    // Authorization scheme
                    Scheme = "Bearer",

                    // Authorization token type 
                    BearerFormat = "JWT",

                    // The location that the token will be send to
                    In = ParameterLocation.Header,

                    // Security protocol type
                    Type = SecuritySchemeType.Http,

                    Description = "Please enter JWT with Bearer into field",
                });

                // Enables the authorization for OpenAPI calls
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme,
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        //  To be called by Startup.ConfigureServices, for adding the requests from PowerServer OpenAPI module in the HTTP request pipeline
        public static IApplicationBuilder UsePowerServerOpenAPI(this IApplicationBuilder app)
        {
            // Sets the HTTP request pipeline
            app.UseSwagger();

            var path = Path.Combine(AppContext.BaseDirectory, "openapi", "openapi-ui");

            // Adds the file server to access the OpenAPI output UI
            app.UseFileServer(new FileServerOptions()
            {
                // Configures the path to the openapi-ui 
                RequestPath = "/openapi-ui",

                // Sets the physical directory in which the file server provides the file services
                FileProvider = new PhysicalFileProvider(path)
            }) ;

            // Enables Swagger UI for visual debugging
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ServerAPIs v1");

                options.DocumentTitle = "PowerServer API";

                options.RoutePrefix = "PowerServer";
                
                options.InjectStylesheet("/openapi-ui/css/custom-stylesheet.css");

                options.InjectJavascript("/openapi-ui/lib/custom-javascript.js");
            });

            return app;
        }
    }
}

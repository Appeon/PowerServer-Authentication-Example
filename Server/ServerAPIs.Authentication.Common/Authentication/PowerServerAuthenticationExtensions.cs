using Microsoft.Extensions.Configuration;
using ServerAPIs.Authentication.Common;
using System.Net;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PowerServerAuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationPlatform(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<TokenClient>().ConfigurePrimaryHttpMessageHandler(provider =>
            {
                return new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = delegate { return true; },
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
            });

            // Add a default authentication scheme. The authentication handler of this scheme can parse the request header set by the client to select the appropriate authentication scheme
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = "Default";
            }).AddScheme<DefaultAuthenticationOptions, DefaultAuthenticationHandlerPlatform>("Default", opts => { });

            services.AddSingleton(new AesUtilities(configuration));

            return services;
        }
    }
}

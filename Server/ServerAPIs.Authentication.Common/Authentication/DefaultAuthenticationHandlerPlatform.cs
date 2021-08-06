using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.Common
{
    public class DefaultAuthenticationOptions : AuthenticationSchemeOptions
    {
        public Func<HttpRequest, string> TokenRetriever { get; set; } = TokenRetrieval.FromAuthorizationHeader();

    }

    public class DefaultAuthenticationHandlerPlatform : AuthenticationHandler<DefaultAuthenticationOptions>
    {
        private readonly ILogger _logger;

        public DefaultAuthenticationHandlerPlatform(
            IOptionsMonitor<DefaultAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<DefaultAuthenticationHandlerPlatform>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Options.TokenRetriever(Context.Request);

            try
            {
                AuthenticateResult authResult;
                if (token != null)
                {
                    // Set the "Authentication-Provider" HTTP request header in PB code to distinguish different authentication schemes
                    if (Context.Request.Headers.TryGetValue("Authentication-Provider", out var provider))
                    {
                        var scheme = provider.ToString();

                        authResult = await Context.AuthenticateAsync(scheme);

                        if (!authResult.Succeeded)
                        {
                            return AuthenticateResult.Fail("Unauthorized");
                        }

                        return AuthenticateResult.Success(new AuthenticationTicket(authResult.Principal, scheme));
                    }
                    return AuthenticateResult.Fail("Unauthorized");
                }

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return AuthenticateResult.Fail(ex);
            }
        }
    }
}

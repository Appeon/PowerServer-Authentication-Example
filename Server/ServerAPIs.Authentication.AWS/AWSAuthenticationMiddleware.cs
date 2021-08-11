using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ServerAPIs.Authentication.AWS;
using ServerAPIs.Authentication.Common;
using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public class AWSAuthenticationMiddleware : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);

                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/aws/token", TokenEndpointAsync);        // TokenEndpoint
                    endpoints.MapGet("/aws/callback", CallbackEndpointAsync);   // CallbackUrl
                    endpoints.MapPost("/aws/user", UserEndpointAsync);          // UserEndpoint
                });
            };
        }

        private async Task TokenEndpointAsync(HttpContext context)
        {
            try
            {
                var userStore = context.RequestServices.GetRequiredService<AWSUserStore>();

                using var streamReader = new StreamReader(context.Request.Body);
                var content = await streamReader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<TokenRequest>(content, new JsonSerializerSettings() { }) ?? new TokenRequest();

                var response = await userStore.ValidateAsync(request.Username, request.Password);

                if (response != null)
                {
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    var responseToken = new TokenResponse()
                    {
                        AccessToken = response.AuthenticationResult.AccessToken,
                        ExpireIn = response.AuthenticationResult.ExpiresIn,
                        TokenType = response.AuthenticationResult.TokenType
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(responseToken), Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsync(ex.Message, Encoding.UTF8);
            }
        }

        // The authentication flow based on authorization_code mode will callback to this method
        private async Task CallbackEndpointAsync(HttpContext context)
        {
            var userStore = context.RequestServices.GetRequiredService<AWSUserStore>();
            var tokenManager = context.RequestServices.GetRequiredService<CognitoTokenManager>();

            var code = context.Request.QueryString.ToString()[6..];

            var result = await userStore.ValidateByAuthorizationCodeAsync(code);

            tokenManager.Add(result.Item1, code, result.Item2);
        }

        private async Task UserEndpointAsync(HttpContext context)
        {
            var tokenManager = context.RequestServices.GetRequiredService<CognitoTokenManager>();

            using var streamReader = new StreamReader(context.Request.Body);
            var content = await streamReader.ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<UserRequest>(content, new JsonSerializerSettings() { }) ?? new UserRequest();

            var ciphertext = request.Content;

            var id = AesUtilities.Decrypt(ciphertext);

            var token = tokenManager.Tokens[id];

            await context.Response.WriteAsync(token.AccessToken, Encoding.UTF8);
        }
    }
}

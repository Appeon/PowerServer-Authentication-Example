using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ServerAPIs.Authentication.Common;
using ServerAPIs.Authentication.Jwt;
using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public class JwtAuthenticationMiddleware : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);

                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/jwt/token", TokenEndpointAsync); // TokenEndpoint
                });
            };
        }

        private async Task TokenEndpointAsync(HttpContext context)
        {
            try
            {
                var userStore = context.RequestServices.GetRequiredService<JwtUserStore>();
                var tokenHandler = context.RequestServices.GetRequiredService<JwtHandler>();

                using var streamReader = new StreamReader(context.Request.Body);
                var content = await streamReader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<TokenRequest>(content, new JsonSerializerSettings() { }) ?? new TokenRequest();

                if (await userStore.ValidateAsync(request.Username, request.Password) is JwtUser user)
                {
                    var response = tokenHandler.CreateToken(user);

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response), Encoding.UTF8);
                }
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsync("Invalid login attempt.", Encoding.UTF8);
            }
        }
    }
}

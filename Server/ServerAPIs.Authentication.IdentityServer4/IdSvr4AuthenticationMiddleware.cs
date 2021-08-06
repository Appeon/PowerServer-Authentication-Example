using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerAPIs.Authentication.Common;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.IdentityServer4
{
    public class IdSvr4AuthenticationMiddleware : IStartupFilter
    {
        private readonly HttpClient _httpClient;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _scope;

        public IdSvr4AuthenticationMiddleware(TokenClient tokenClient, IOptionsMonitor<IdSvr4Options> opts)
        {
            _clientId = opts.CurrentValue.ClientID;
            _clientSecret = opts.CurrentValue.ClientSecret;
            _scope = opts.CurrentValue.Scope;

            _httpClient = tokenClient.Client;
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_clientId + ":" + _clientSecret)));
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseIdentityServer();

                next(builder);

                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/idsvr4/token", TokenEndpointAsync); // TokenEndpoint
                });
            };
        }

        private async Task TokenEndpointAsync(HttpContext context)
        {
            using var streamReader = new StreamReader(context.Request.Body);
            var body = await streamReader.ReadToEndAsync();
            var login = JsonConvert.DeserializeObject<TokenRequest>(body, new JsonSerializerSettings() { }) ?? new TokenRequest();

            var requestUri = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/connect/token";

            var content = UrlEncodedContentBuilder.BuildEncodedContent(GrantType.Password, login.Username, login.Password, _scope);

            var response = await _httpClient.PostAsync(requestUri, content);

            var jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());

            var error = jsonObj.Value<string>("error");

            if (!string.IsNullOrWhiteSpace(error))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsync(error, Encoding.UTF8);
            }
            else
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(jsonObj), Encoding.UTF8);
            }
        }
    }
}

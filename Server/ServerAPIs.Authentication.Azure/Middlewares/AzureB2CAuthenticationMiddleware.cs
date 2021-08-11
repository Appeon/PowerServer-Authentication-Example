﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerAPIs.Authentication.Azure;
using ServerAPIs.Authentication.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public class AzureB2CAuthenticationMiddleware : IStartupFilter
    {
        private readonly HttpClient _httpClient;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _ropc_tokenEndpoint;
        private readonly string _b2c_tokenEndpoint;
        private readonly string _scope;
        private readonly string _redirectUri;

        public AzureB2CAuthenticationMiddleware(TokenClient tokenClient, IOptionsMonitor<AzureB2COptions> opts)
        {
            _clientId = opts.CurrentValue.ClientID;
            _clientSecret = opts.CurrentValue.ClientSecret;
            _ropc_tokenEndpoint = opts.CurrentValue.ROPCTokenEndpoint;
            _b2c_tokenEndpoint = opts.CurrentValue.B2CTokenEndpoint;
            _scope = opts.CurrentValue.Scope;
            _redirectUri = opts.CurrentValue.RedirectUri;

            _httpClient = tokenClient.Client;
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_clientId + ":" + _clientSecret)));
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);

                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost("/azureb2c/token", TokenEndpointAsync);       // TokenEndpoint
                    endpoints.MapGet("/azureb2c/callback", CallbackEndpointAsync);  // CallbackUrl
                    endpoints.MapPost("/azureb2c/user", UserEndpointAsync);         // UserEndpoint
                });
            };
        }

        private async Task TokenEndpointAsync(HttpContext context)
        {
            using var streamReader = new StreamReader(context.Request.Body);
            var body = await streamReader.ReadToEndAsync();
            var login = JsonConvert.DeserializeObject<TokenRequest>(body, new JsonSerializerSettings() { }) ?? new TokenRequest();

            var parms = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "client_id", _clientId },
                { "username", login.Username },
                { "password", login.Password },
                { "scope", _scope }
            };

            var response = await _httpClient.PostAsync(_ropc_tokenEndpoint, new FormUrlEncodedContent(parms));
            var jsonObj = JObject.Parse(await response.Content.ReadAsStringAsync());

            context.Response.ContentType = MediaTypeNames.Application.Json;

            var text = JsonConvert.SerializeObject(jsonObj);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var error = jsonObj.Value<string>("error");

                await context.Response.WriteAsync(error, Encoding.UTF8);
            }
            else
            {
                await context.Response.WriteAsync(text, Encoding.UTF8);
            }
        }

        // The authentication flow based on authorization_code mode will callback to this method
        private async Task CallbackEndpointAsync(HttpContext context)
        {
            var tokenManager = context.RequestServices.GetRequiredService<AzureB2CTokenManager>();

            var code = context.Request.QueryString.ToString()[6..];

            var jwt = await ValidateByAuthorizationCodeAsync(code);

            tokenManager.Add(jwt.Item1, code, jwt.Item2);
        }

        private async Task UserEndpointAsync(HttpContext context)
        {
            var tokenManager = context.RequestServices.GetRequiredService<AzureB2CTokenManager>();

            using var streamReader = new StreamReader(context.Request.Body);
            var content = await streamReader.ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<UserRequest>(content, new JsonSerializerSettings() { }) ?? new UserRequest();

            var ciphertext = request.Content;

            var id = AesUtilities.Decrypt(ciphertext);

            var token = tokenManager.Tokens[id];

            await context.Response.WriteAsync(token.AccessToken, Encoding.UTF8);
        }

        private async Task<(string, TokenResponse)> ValidateByAuthorizationCodeAsync(string code)
        {
            var parms = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "redirect_uri", _redirectUri },
                { "scope", _scope }
            };

            var response = await _httpClient.PostAsync(_b2c_tokenEndpoint, new FormUrlEncodedContent(parms));
            var result = await response.Content.ReadAsStringAsync();

            var dynamicObj = JObject.Parse(result);

            var token = dynamicObj.Value<string>("access_token");
            var expireIn = dynamicObj.Value<int>("expires_in");
            var tokenType = dynamicObj.Value<string>("token_type");

            var jwtSecurityAccessToken = new JwtSecurityToken(token);
            var claims_AccessToken = jwtSecurityAccessToken.Claims;

            var userId = claims_AccessToken.FirstOrDefault(t => t.Type.Equals("emails", StringComparison.OrdinalIgnoreCase))?.Value ?? throw new ArgumentNullException("Can't locate the user's Id");

            var tokenResponse = new TokenResponse()
            {
                AccessToken = token,
                ExpireIn = expireIn,
                TokenType = tokenType
            };
            return (userId, tokenResponse);
        }
    }
}

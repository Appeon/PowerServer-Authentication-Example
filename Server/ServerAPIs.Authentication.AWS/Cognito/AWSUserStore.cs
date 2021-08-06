using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerAPIs.Authentication.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.AWS
{
    public class AWSUserStore : UserStoreBase<AWSUser>
    {
        private readonly CognitoUserPool _pool;
        private readonly string redirect_uri;
        private readonly string client_secret;

        public AWSUserStore(ILogger<AWSUserStore> logger, IConfiguration configuration)
        {
            base.logger = logger;

            redirect_uri = configuration["AWS:CallbackUrl"];
            client_secret = configuration["AWS:UserPoolClientSecret"];

            var cognitoClientOptions = configuration.GetAWSCognitoClientOptions();
            var cognitoIdentityClient = new AmazonCognitoIdentityProviderClient(
                new AnonymousAWSCredentials(), configuration.GetAWSOptions().Region);

            _pool = new CognitoUserPool(
                cognitoClientOptions.UserPoolId,
                cognitoClientOptions.UserPoolClientId,
                cognitoIdentityClient,
                cognitoClientOptions.UserPoolClientSecret);
        }

        public override async Task<AWSUser> ValidateAsync(string username, string password)
        {
            try
            {
                var user = _pool.GetUser(username);

                var authResponse = await user.StartWithSrpAuthAsync(
                    new InitiateSrpAuthRequest { Password = password });

                if (user.SessionTokens.IsValid())
                {
                    logger.LogInformation($"User <{username}> logged in.");

                    return new AWSUser
                    {
                        AuthenticationResult = authResponse.AuthenticationResult
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            logger.LogError($"Invalid login attempt.");

            return default;
        }

        // Get token in authorization_code mode，return userId、AuthenticationResultType
        public async Task<(string, AuthenticationResultType)> ValidateByAuthorizationCodeAsync(string code)
        {
            var parms = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "client_id", _pool.ClientID },
                { "code", code },
                { "redirect_uri", redirect_uri },
            };

            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(_pool.ClientID + ":" + client_secret));

            var request = WebRequest.CreateHttp("https://<you domain>.auth.<your region>.amazoncognito.com/oauth2/token");

            request.ProtocolVersion = HttpVersion.Version11;
            request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(
                (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                {
                    return true;
                });

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + basicAuth);

            var buffer = new StringBuilder();

            var i = 0;
            foreach (var key in parms.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parms[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parms[key]);
                }
                i++;
            }

            var dataBytes = Encoding.UTF8.GetBytes(buffer.ToString());

            request.ContentLength = dataBytes.Length;

            using var dataStream = request.GetRequestStream();
            dataStream.Write(dataBytes, 0, dataBytes.Length);

            try
            {
                using var response = await request.GetResponseAsync() as HttpWebResponse;

                string sessionJson;
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    using var responseReader = new StreamReader(response.GetResponseStream());
                    sessionJson = await responseReader.ReadToEndAsync();

                    var dynamicSessionObj = JsonConvert.DeserializeObject(sessionJson) as JObject ?? throw new Exception();

                    var authResult = new AuthenticationResultType()
                    {
                        IdToken = dynamicSessionObj.Value<string>("id_token"),
                        AccessToken = dynamicSessionObj.Value<string>("access_token"),
                        RefreshToken = dynamicSessionObj.Value<string>("refresh_token"),
                        ExpiresIn = dynamicSessionObj.Value<int>("expires_in"),
                        TokenType = dynamicSessionObj.Value<string>("token_type"),
                    };

                    var jwtSecurityIdToken = new JwtSecurityToken(authResult.IdToken);
                    var jwtSecurityAccessToken = new JwtSecurityToken(authResult.AccessToken);

                    var claims_IdToken = jwtSecurityIdToken.Claims;
                    var claims_AccessToken = jwtSecurityAccessToken.Claims;

                    var userId = claims_IdToken.FirstOrDefault(t => t.Type.Equals("email", StringComparison.OrdinalIgnoreCase))?.Value ?? throw new ArgumentNullException("Can't locate the user's Id");

                    return (userId, authResult);
                }
            }
            catch (Exception)
            {
                throw new NotAuthorizedException("Authentication failed.");
            }
            return default;
        }
    }
}

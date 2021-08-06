using System.Collections.Generic;
using System.Net.Http;

namespace ServerAPIs.Authentication.Common
{
    internal enum GrantType
    {
        Authorization_code,
        Client_credentials,
        Password,
        Implicit
    }

    // This class is used to build form data whose "Content-Type" is "x-www-form-urlencoded"
    internal static class UrlEncodedContentBuilder
    {
        internal static FormUrlEncodedContent BuildEncodedContent(
            GrantType grantType,
            string username = null, string password = null, string scope = null)
        {
            var data = grantType switch
            {
                GrantType.Authorization_code => AuthorizationCodeData(username, password, scope),
                GrantType.Password => PasswordData(username, password, scope),
                GrantType.Client_credentials => ClientCredentialsData(scope),
                GrantType.Implicit => throw new System.NotSupportedException(),
                _ => throw new System.NotSupportedException()
            };

            return new FormUrlEncodedContent(data);
        }
#nullable enable
        private static IEnumerable<KeyValuePair<string, string>> AuthorizationCodeData(
            string username, string password, string? scope = null)
        {
            return new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "username", username },
                { "password", password },
                { "scope", scope ?? string.Empty }
            };
        }

        private static IEnumerable<KeyValuePair<string, string>> PasswordData(
            string username, string password, string? scope = null)
        {
            return new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password },
                { "scope", scope ?? string.Empty }
            };
        }

        private static IEnumerable<KeyValuePair<string, string>> ClientCredentialsData(string? scope = null)
        {
            return new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", scope ?? string.Empty }
            };
        }
    }
#nullable disable
}

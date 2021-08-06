using Newtonsoft.Json;

namespace ServerAPIs.Authentication.Common
{
    public class TokenResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpireIn { get; set; } = 3600;
    }
}

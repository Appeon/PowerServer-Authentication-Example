using ServerAPIs.Authentication.Common;

namespace ServerAPIs.Authentication.Azure
{
    public class AzureADOptions : ICredentialsOptions
    {
        public const string Position = "AzureAD";

        public string ClientID { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string TokenEndpoint { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;
    }
}

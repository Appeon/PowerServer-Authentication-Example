using ServerAPIs.Authentication.Common;

namespace ServerAPIs.Authentication.Azure
{
    public class AzureB2COptions : ICredentialsOptions
    {
        public const string Position = "AzureB2C";

        public string ClientID { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string ROPCTokenEndpoint { get; set; } = string.Empty;

        public string B2CTokenEndpoint { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public string RedirectUri { get; set; } = string.Empty;
    }
}

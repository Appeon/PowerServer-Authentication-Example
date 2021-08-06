using ServerAPIs.Authentication.Common;

namespace ServerAPIs.Authentication.IdentityServer4
{
    public class IdSvr4Options : ICredentialsOptions
    {
        public const string Position = "IdSvr4";

        public string ClientID { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;
    }
}

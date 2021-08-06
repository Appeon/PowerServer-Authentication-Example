namespace ServerAPIs.Authentication.Common
{
    public interface ICredentialsOptions
    {
        string ClientID { get; set; }

        string ClientSecret { get; set; }
    }
}

namespace ServerAPIs.Authentication.Common
{
    public class TokenRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}

using System.Collections.Generic;
using System.Security.Claims;

namespace ServerAPIs.Authentication.Common
{
    public abstract class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}

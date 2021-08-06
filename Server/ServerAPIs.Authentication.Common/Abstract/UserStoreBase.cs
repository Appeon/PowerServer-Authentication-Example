using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.Common
{
    // This abstract class is used for user storage and authentication of all authentication schemes
    public abstract class UserStoreBase<TUser>
        where TUser : User
    {
        protected ILogger logger;
        protected List<TUser> users;

        public abstract Task<TUser> ValidateAsync(string username, string password);
    }
}

using System.Collections.Generic;

namespace ServerAPIs.Authentication.Common
{
    // This interface is used for the authentication flow of authorization_code mode to store authorization_code and token
    public interface ITokenManager<T>
    {
        IDictionary<string, T> Tokens { get; }

        IDictionary<string, string> AuthorizationCodes { get; }

        void Add(string id, string code, T session);
    }
}

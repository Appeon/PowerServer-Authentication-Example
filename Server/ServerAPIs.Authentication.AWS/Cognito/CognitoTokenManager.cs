using Amazon.CognitoIdentityProvider.Model;
using ServerAPIs.Authentication.Common;
using System.Collections.Generic;

namespace ServerAPIs.Authentication.AWS
{
    public class CognitoTokenManager : ITokenManager<AuthenticationResultType>
    {
        public IDictionary<string, AuthenticationResultType> Tokens { get; private set; }

        public IDictionary<string, string> AuthorizationCodes { get; private set; }

        public CognitoTokenManager()
        {
            Tokens = new Dictionary<string, AuthenticationResultType>();
            AuthorizationCodes = new Dictionary<string, string>();
        }

        public void Add(string id, string code, AuthenticationResultType session)
        {
            if (Tokens.ContainsKey(id))
            {
                Tokens[id] = session;
            }
            else
            {
                Tokens.Add(id, session);
            }

            if (AuthorizationCodes.ContainsKey(id))
            {
                AuthorizationCodes[id] = code;
            }
            else
            {
                AuthorizationCodes.Add(id, code);
            }
        }
    }
}

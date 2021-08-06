using ServerAPIs.Authentication.Common;
using System.Collections.Generic;

namespace ServerAPIs.Authentication.Azure
{
    public class AzureB2CTokenManager : ITokenManager<TokenResponse>
    {
        public IDictionary<string, TokenResponse> Tokens { get; private set; }

        public IDictionary<string, string> AuthorizationCodes { get; private set; }

        public AzureB2CTokenManager()
        {
            Tokens = new Dictionary<string, TokenResponse>();
            AuthorizationCodes = new Dictionary<string, string>();
        }

        public void Add(string id, string code, TokenResponse session)
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

using Amazon.CognitoIdentityProvider.Model;
using ServerAPIs.Authentication.Common;

namespace ServerAPIs.Authentication.AWS
{
    public class AWSUser : User
    {
        public AuthenticationResultType AuthenticationResult { get; set; }
    }
}

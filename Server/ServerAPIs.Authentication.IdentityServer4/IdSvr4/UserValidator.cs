using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Threading.Tasks;

namespace ServerAPIs.Authentication.IdentityServer4
{
    public class UserValidator : IResourceOwnerPasswordValidator
    {
        private readonly IdSvr4UserStore _users;

        public UserValidator(IdSvr4UserStore users)
        {
            _users = users;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                if (await _users.ValidateAsync(context.UserName, context.Password) is IdSvr4User { IsActive: true } user)
                {
                    context.Result = new GrantValidationResult(
                        user.SubjectId ?? throw new ArgumentException("Subject ID not set", nameof(user.SubjectId)),
                        OidcConstants.AuthenticationMethods.Password,
                        user.Claims);
                }
                else
                {
                    context.Result = new GrantValidationResult(
                        TokenRequestErrors.InvalidGrant, "Incorrect username or password.");
                }
            }
            catch
            { }
        }
    }
}

using ServerAPIs.Authentication.Common;

namespace ServerAPIs.Authentication.IdentityServer4
{
    public class IdSvr4User : User
    {
        public string SubjectId { get; set; }

        public string ProviderName { get; set; }

        public string ProviderSubjectId { get; set; }

        public bool IsActive { get; set; }
    }
}

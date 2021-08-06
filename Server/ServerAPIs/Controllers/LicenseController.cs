using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PowerServer.Core;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ServerAPIs
{
    /// <summary>
    /// License information query API
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]

    // Sets the authorization policy
    // The default policy is the one used by PowerServer Web APIs
    // For security concern, it is recommended to use different authorization policies for PowerServer Web APIs and management APIs
    // see https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies
    [Authorize(PowerServerConstants.DefaultAuthorizePolicy)]

    // Sets the return data type of the API
    [Produces(MediaTypeNames.Application.Json)]
    // Sets the data type when the API returns error
    [ProducesErrorResponseType(typeof(ValidationProblemDetails))]

    // Sets the possible response statuses and their data types of the API
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public class LicenseController : ControllerBase
    {
        private readonly IPowerServerLicenseClient _licenseClient;
        private readonly IHostApplicationLifetime _hostingLifetime;

        public LicenseController(IPowerServerService service, IHostApplicationLifetime hostingLifetime)
        {
            _hostingLifetime = hostingLifetime;
            _licenseClient = service.LicenseClient;
        }

        /// <summary>
        /// Loads the license information
        /// </summary>
        /// <returns></returns>
        // GET api/license/loadlicenseinfo
        [HttpGet]
        public Task<LicenseQueryResult> LoadLicenseInfoAsync()
        {
            return _licenseClient.GetLicenseAsync(_hostingLifetime.ApplicationStopping);
        }

        /// <summary>
        /// Loads the instance information
        /// </summary>
        /// <returns></returns>
        // GET api/license/loadinstanceinfo
        [HttpGet]
        public Task<InstanceQueryResult> LoadInstanceInfoAsync()
        {
            return _licenseClient.GetInstanceAsync(_hostingLifetime.ApplicationStopping);
        }
    }
}
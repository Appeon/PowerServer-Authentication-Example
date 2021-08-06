using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PowerServer.Core;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ServerAPIs
{
    /// <summary>
    /// Session management API
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
    public class SessionController : ControllerBase
    {
        private readonly IPowerServerSessionClient _sessionClient;
        private readonly IHostApplicationLifetime _hostingLifetime;

        public SessionController(IPowerServerService service, IHostApplicationLifetime hostingLifetime)
        {
            _hostingLifetime = hostingLifetime;
            _sessionClient = service.SessionClient;
        }

        /// <summary>
        /// Loads all session information
        /// </summary>
        /// <returns></returns>
        // GET api/session/loadall
        [HttpGet]
        public Task<IList<SessionQueryResult>> LoadAllAsync()
        {
            return _sessionClient.GetAllSessionAsync(_hostingLifetime.ApplicationStopping);
        }

        /// <summary>
        /// Gets the session number of the current server instance
        /// </summary>
        /// <returns></returns>
        // GET api/session/getsessioncount
        [HttpGet]
        public Task<int> GetSessionCountAsync()
        {
            return _sessionClient.GetSessionCountAsync(_hostingLifetime.ApplicationStopping);
        }

        /// <summary>
        /// Kill a given session
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns></returns>
        // POST api/session/killbyid/5
        [HttpPost("{sessionId}")]
        public Task<ISessionResult> KillByIdAsync(string sessionId)
        {
            return _sessionClient.KillSessionAsync(sessionId, _hostingLifetime.ApplicationStopping);
        }
    }
}
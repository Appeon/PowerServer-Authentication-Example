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
    /// Connection configuration management API
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
    public class ConnectionController : ControllerBase
    {
        // PowerServer lifetime object
        private readonly IHostApplicationLifetime _hostingLifetime;
        // PowerServer configuration client
        private readonly IPowerServerConfigurationClient _configurationClient;

        public ConnectionController(IPowerServerService service, IHostApplicationLifetime hostingLifetime)
        {
            _hostingLifetime = hostingLifetime;
            _configurationClient = service.ConfigurationClient;
        }

        /// <summary>
        /// Loads the specified connection configuration
        /// </summary>
        /// <param name="cacheGroup">CacheGroup</param>
        /// <param name="cacheName">Cache name</param>
        /// <returns></returns>
        // GET api/connection/loadone/foo/bar
        [HttpGet("{cacheGroup}/{cacheName}")]
        public Task<ConnectionConfigurationItem> LoadOneAsync(string cacheGroup, string cacheName)
        {
            return _configurationClient.GetConnectionAsync(cacheGroup, cacheName, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Loads the specified CacheGroup configuration
        /// </summary>
        /// <param name="cacheGroup">CacheGroup</param>
        /// <returns></returns>
        // GET api/connection/loadgroup/foo
        [HttpGet("{cacheGroup}")]
        public Task<IEnumerable<ConnectionConfigurationItem>> LoadGroupAsync(string cacheGroup)
        {
            return _configurationClient.GetConnectionsAsync(cacheGroup, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Loads all the connection configuration
        /// </summary>
        /// <returns></returns>
        // GET api/connection/loadall
        [HttpGet]
        public Task<IEnumerable<ConnectionConfigurationGroup>> LoadAllAsync()
        {
            return _configurationClient.GetConnectionsAsync(_hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Adds a connection configuration
        /// </summary>
        /// <param name="cacheGroup"> CacheGroup</param>
        /// <param name="item">Item</param>
        /// <returns></returns>
        // POST api/connection/addone/foo
        [HttpPost("{cacheGroup}")]
        public Task<ConfigurationUpdateResult> AddOneAsync(string cacheGroup, [FromBody] ConnectionConfigurationItem item)
        {
            return _configurationClient.AddConnectionAsync(cacheGroup, item, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Adds a group of connection configuration
        /// </summary>
        /// <param name="cacheGroup">CacheGroup</param>
        /// <param name="items">Items</param>
        /// <returns></returns>
        // POST api/connection/addrange/foo
        [HttpPost("{cacheGroup}")]
        public Task<ConfigurationUpdateResult> AddRangeAsync(string cacheGroup, [FromBody] IEnumerable<ConnectionConfigurationItem> items)
        {
            return _configurationClient.AddConnectionAsync(cacheGroup, items, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Adds a group of empty connection configuration, and copies connection configuration list from the specified CacheGroup
        /// </summary>
        /// <param name="cacheGroup">New CacheGroup</param>
        /// <param name="copyFrom">Old CacheGroup</param>
        /// <returns></returns>
        // POST api/connection/addgroup/foo/bar
        [HttpPost("{cacheGroup}/{copyFrom}")]
        public Task<ConfigurationUpdateResult> AddGroupAsync(string cacheGroup, string copyFrom)
        {
            return _configurationClient.AddGroupConnectionAsync(cacheGroup, copyFrom, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Edit a connection configuration
        /// </summary>
        /// <param name="cacheGroup">CacheGroup</param>
        /// <param name="item">Item</param>
        /// <returns></returns>
        // POST api/connection/edit/foo
        [HttpPost("{cacheGroup}")]
        public Task<ConfigurationUpdateResult> EditAsync(string cacheGroup, [FromBody] ConnectionConfigurationItem item)
        {
            return _configurationClient.EditConnectionAsync(cacheGroup, item, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Removes a connection configuration
        /// </summary>
        /// <param name="cacheGroup">CacheGroup</param>
        /// <param name="cacheName">Cache name</param>
        /// <returns></returns>
        // POST api/connection/removeone/foo/bar
        [HttpPost("{cacheGroup}/{cacheName}")]
        public Task<ConfigurationUpdateResult> RemoveOneAsync(string cacheGroup, string cacheName)
        {
            return _configurationClient.RemoveConnectionAsync(cacheGroup, cacheName, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Removes a CacheGroup and the connection configuration in it
        /// </summary>
        /// <param name="cacheGroup">CacheGroup</param>
        /// <returns></returns>
        // POST api/connection/removegroup/foo/bar
        [HttpPost("{cacheGroup}")]
        public Task<ConfigurationUpdateResult> RemoveGroupAsync(string cacheGroup)
        {
            return _configurationClient.RemoveGroupConnectionAsync(cacheGroup, _hostingLifetime.ApplicationStopped);
        }
    }
}

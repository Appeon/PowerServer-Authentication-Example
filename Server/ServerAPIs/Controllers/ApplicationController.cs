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
    /// Application configuration management APIs
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
    public class ApplicationController : ControllerBase
    {
        // PowerServer lifetime object
        private readonly IHostApplicationLifetime _hostingLifetime;
        // PowerServer configuration client
        private readonly IPowerServerConfigurationClient _configurationClient;

        public ApplicationController(IPowerServerService service, IHostApplicationLifetime hostingLifetime)
        {
            _hostingLifetime = hostingLifetime;
            _configurationClient = service.ConfigurationClient;
        }

        /// <summary>
        /// Loads the specified application configuration
        /// </summary>
        /// <param name="appName">application name</param>
        /// <returns></returns>
        // GET api/application/loadone/foo
        [HttpGet("{appName}")]
        public Task<ApplicationConfigurationItem> LoadOneAsync(string appName)
        {
            return _configurationClient.GetApplicationAsync(appName, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Loads all application configuration
        /// </summary>
        /// <returns></returns>
        // GET api/application/loadall
        [HttpGet]
        public Task<IEnumerable<ApplicationConfigurationItem>> LoadAllAsync()
        {
            return _configurationClient.GetApplicationsAsync(_hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Adds application configuration 
        /// </summary>
        /// <param name="item">The configuration to be added</param>
        /// <returns></returns>
        // POST api/application/add
        [HttpPost]
        public Task<ConfigurationUpdateResult> AddAsync([FromBody] ApplicationConfigurationItem item)
        {
            return _configurationClient.AddApplicationAsync(item, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Edits application configuration
        /// </summary>
        /// <param name="item">The configuration to be edited</param>
        /// <returns></returns>
        // POST api/application/edit
        [HttpPost]
        public Task<ConfigurationUpdateResult> EditAsync([FromBody] ApplicationConfigurationItem item)
        {
            return _configurationClient.EditApplicationAsync(item, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Removes application configuration
        /// </summary>
        /// <param name="appName">The name of the application to be removed</param>
        /// <returns></returns>
        // POST api/application/remove/foo
        [HttpPost("{appName}")]
        public Task<ConfigurationUpdateResult> RemoveAsync(string appName)
        {
            return _configurationClient.RemoveApplicationAsync(appName, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Adds transaction mapping
        /// </summary>
        /// <param name="appName">Application name</param>
        /// <param name="transName">Transaction name</param>
        /// <param name="item">Item</param>
        /// <returns></returns>
        // POST api/application/addtransaction/foo/bar
        [HttpPost("{appName}/{transName}")]
        public Task<ConfigurationUpdateResult> AddTransactionMappingAsync(
            string appName, string transName, TransactionConfiguration item)
        {
            return _configurationClient.AddTransactionMappingAsync(
                appName, transName, item, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Edits transaction mapping
        /// </summary>
        /// <param name="appName">Application name</param>
        /// <param name="transName">Transaction name</param>
        /// <param name="item">Item</param>
        /// <returns></returns>
        // POST api/application/edittransaction/foo/bar
        [HttpPost("{appName}/{transName}")]
        public Task<ConfigurationUpdateResult> EditTransactionMappingAsync(
            string appName, string transName, TransactionConfiguration item)
        {
            return _configurationClient.EditTransactionMappingAsync(
                appName, transName, item, _hostingLifetime.ApplicationStopped);
        }

        /// <summary>
        /// Removes transaction mapping
        /// </summary>
        /// <param name="appName">Application name</param>
        /// <param name="transName">Transaction name</param>
        /// <returns></returns>
        // POST api/application/removetransaction/foo/bar
        [HttpPost("{appName}/{transName}")]
        public Task<ConfigurationUpdateResult> RemoveTransactionMappingAsync(string appName, string transName)
        {
            return _configurationClient.RemoveTransactionMappingAsync(appName, transName, _hostingLifetime.ApplicationStopped);
        }
    }
}

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
    /// Transaction management API
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
    public class TransactionController : ControllerBase
    {
        private readonly IHostApplicationLifetime _hostingLifetime;
        private readonly IPowerServerTransactionClient _transactionClient;

        public TransactionController(IPowerServerService service, IHostApplicationLifetime hostingLifetime)
        {
            _hostingLifetime = hostingLifetime;
            _transactionClient = service.TransactionClient;
        }

        /// <summary>
        /// Loads all transaction data
        /// </summary>
        /// <returns></returns>
        // GET api/transaction/loadall
        [HttpGet]
        public Task<IEnumerable<TransactionQueryResult>> LoadAllAsync()
        {
            return _transactionClient.GetAllAsync(_hostingLifetime.ApplicationStopping);
        }

        /// <summary>
        /// Loads the SQL of the request in the specified transaction
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns></returns>
        // GET api/transaction/loadrequestsql
        [HttpGet("{transactionId}")]
        public Task<IEnumerable<TransactionSqlResult>> LoadRequestSqlAsync(string transactionId)
        {
            return _transactionClient.GetRequestSqlAsync(transactionId, _hostingLifetime.ApplicationStopping);
        }

        /// <summary>
        /// Rolls back the specified transaction
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns></returns>
        // POST api/transaction/rollbackbyid/5/5
        [HttpPost("{sessionId}/{transactionId}")]
        public Task<TransactionRollbackResult> RollbackByIdAsync(string sessionId, string transactionId)
        {
            return _transactionClient.RollbackAsync(sessionId, transactionId, _hostingLifetime.ApplicationStopping);
        }
    }
}
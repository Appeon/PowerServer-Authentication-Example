using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UserExtensions
{
    public class HealthChecksPublisher : IHealthCheckPublisher
    {
        // To be called on schedule if it is injected into HealthChecksExtension.AddPowerServerHealthChecks
        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            // Adds code here to send the health check results to other system or write into database
            // To enable the API, uncomment the relevant script in HealthChecksExtensions.AddServerAPIsHealthChecks 
            // see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks#health-check-publisher

            return Task.CompletedTask;
        }
    }
}

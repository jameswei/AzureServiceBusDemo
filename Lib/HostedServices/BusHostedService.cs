using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lib.HostedServices
{
    public class BusHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IBusControl _busControl;

        public BusHostedService(IBusControl busControl, ILogger logger)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("{name} is starting", nameof(BusHostedService));

            await _busControl.StartAsync(cancellationToken);

            _logger.LogTrace("{name} is started", nameof(BusHostedService));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("{name} is stopping", nameof(BusHostedService));

            await _busControl.StopAsync(cancellationToken);

            _logger.LogTrace("{name} is stopped", nameof(BusHostedService));
        }
    }
}

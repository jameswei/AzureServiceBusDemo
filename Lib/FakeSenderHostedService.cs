using System;
using System.Threading;
using System.Threading.Tasks;
using Lib.Messages;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lib
{
    public class FakeSenderHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IBusControl _busControl;

        public FakeSenderHostedService(IBusControl busControl, ILogger logger)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("{name} is starting", nameof(FakeSenderHostedService));

            for (var index = 0; index < 10; index++)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                await SendFakeMessageAsync();
            }

            _logger.LogTrace("{name} is started", nameof(FakeSenderHostedService));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("{name} is stopping", nameof(FakeSenderHostedService));
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            _logger.LogTrace("{name} is stopped", nameof(FakeSenderHostedService));
        }

        private async Task SendFakeMessageAsync()
        {
            var uniqueId = Guid.NewGuid();
            var message = new SomeMessage
            {
                RequestId = uniqueId,
                Reference = uniqueId.ToString(),
                CreationDate = DateTime.Now
            };

            try
            {
                _logger.LogInformation("Sending message '{uniqueId}'", uniqueId);
                await _busControl.Publish(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send message {ex}", ex);
            }
        }
    }
}

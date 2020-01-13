using System.Threading.Tasks;
using Lib.Consumers.Attributes;
using Lib.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Lib.Consumers
{
    [SomeMessageConsumer]
    public class SomeMessageConsumer : AbstractConsumer<SomeMessage>
    {
        public SomeMessageConsumer(ILogger logger) : base(logger)
        {
        }

        protected override Task ConsumeAsync(ConsumeContext<SomeMessage> context)
        {
            var message = context.Message;
            Logger.LogInformation("Received message with Id '{id}' and Reference '{reference}'", message.RequestId, message.Reference);
            return Task.CompletedTask;
        }
    }
}
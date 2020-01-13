using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Lib.Consumers
{
    public abstract class AbstractConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        protected readonly ILogger Logger;

        protected AbstractConsumer(ILogger logger)
        {
            Logger = logger;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            try
            {
                await ConsumeAsync(context);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Exception thrown on Consumer {consumer} for Message {type}", GetType().Name, typeof(TMessage));
            }
        }

        protected abstract Task ConsumeAsync(ConsumeContext<TMessage> context);
    }
}

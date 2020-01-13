using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Lib.Observers
{
    public class BusReceiveObserver : IReceiveObserver
    {
        private readonly ILogger _logger;

        public BusReceiveObserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BusReceiveObserver>();
        }

        public BusReceiveObserver(ILogger logger)
        {
            _logger = logger;
        }

        public Task PreReceive(ReceiveContext context)
        {
            return LogObserverInfo(context);
        }

        public Task PostReceive(ReceiveContext context)
        {
            return LogObserverInfo(context);
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            return LogObserverInfo(context?.ReceiveContext, consumerType);
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            return LogObserverException(exception, consumerType);
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            return LogObserverException(exception);
        }

        
        private Task LogObserverInfo(ReceiveContext context, string consumerType = null, CallerMemberNameAttribute caller = null)
        {
            if (consumerType == null)
            {
                _logger.LogDebug("{observer} : {caller} - {address}", nameof(BusObserver), caller, context.InputAddress);
            }
            else
            {
                _logger.LogDebug("{observer} : {caller} - {consumerType} - {address}", nameof(BusObserver), caller, consumerType, context.InputAddress);
            }
            
            return Task.CompletedTask;
        }

        private Task LogObserverException(Exception ex, string consumerType = null, CallerMemberNameAttribute caller = null)
        {
            if (consumerType == null)
            {
                _logger.LogError("{observer} : {caller} - {exception}", nameof(BusObserver), caller, ex);
            }
            else
            {
                _logger.LogError("{observer} : {caller} - {consumerType} - {exception}", nameof(BusObserver), caller, consumerType, ex);
            }

            return Task.CompletedTask;
        }
    }
}

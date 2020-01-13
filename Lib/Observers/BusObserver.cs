using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Lib.Observers
{
    public class BusObserver : IBusObserver
    {
        private readonly ILogger _logger;

        public BusObserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BusObserver>();
        }

        public BusObserver(ILogger logger)
        {
            _logger = logger;
        }

        public Task PostCreate(IBus bus)
        {
            return LogObserverInfo(bus);
        }

        public Task CreateFaulted(Exception exception)
        {
            return LogObserverException(exception);
        }

        public Task PreStart(IBus bus)
        {
            return LogObserverInfo(bus);
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return LogObserverInfo(bus);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return LogObserverInfo(bus);
        }

        public Task PreStop(IBus bus)
        {
            return LogObserverInfo(bus);
        }

        public Task PostStop(IBus bus)
        {
            return LogObserverInfo(bus);
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return LogObserverException(exception);
        }

        private Task LogObserverInfo(IBus bus, CallerMemberNameAttribute caller = null)
        {
            _logger.LogDebug("{observer} : {caller} - {address}", nameof(BusObserver), caller, bus.Address);
            return Task.CompletedTask;
        }

        private Task LogObserverException(Exception ex, CallerMemberNameAttribute caller = null)
        {
            _logger.LogError("{observer} : {caller} - {exception}", nameof(BusObserver), caller, ex);
            return Task.CompletedTask;
        }
    }
}

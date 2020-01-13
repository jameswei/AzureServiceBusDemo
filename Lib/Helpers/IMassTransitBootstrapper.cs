using MassTransit;

namespace Lib.Helpers
{
    public interface IMassTransitBootstrapper
    {
        IBusControl CreateBusControl();
    }
}
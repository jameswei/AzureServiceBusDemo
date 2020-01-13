using MassTransit;

namespace Lib.Bootstrapping
{
    public interface IMassTransitBootstrapper
    {
        IBusControl CreateBusControl();
    }
}
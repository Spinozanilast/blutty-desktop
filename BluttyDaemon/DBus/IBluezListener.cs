using HashtagChris.DotNetBlueZ;

namespace BluttyDaemon;

public interface IBluezListener
{
    public Task StartDiscoveryAsync(DeviceChangeEventHandlerAsync deviceFoundHandler);
    public Task StopDiscoveryAsync();
}
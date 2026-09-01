using BluttyDaemon.Exceptions;
using HashtagChris.DotNetBlueZ;

namespace BluttyDaemon;

public class BluezDBusListener : IBluezListener
{
    public static async Task<BluezDBusListener> PrepareDefaultAsync()
    {
        var accessibleAdapters = await BlueZManager.GetAdaptersAsync();
        return accessibleAdapters.Count == 0
            ? throw new NotAnyDeviceAccessibleException()
            : new BluezDBusListener(accessibleAdapters[0]);
    }

    private readonly Adapter _adapter;

    public BluezDBusListener(IAdapter1 adapter)
    {
        _adapter = (Adapter)adapter;
    }

    public async Task StartDiscoveryAsync(DeviceChangeEventHandlerAsync deviceFoundHandler)
    {
        _adapter.DeviceFound += deviceFoundHandler;
        await _adapter.StartDiscoveryAsync();
    }

    public async Task StopDiscoveryAsync()
    {
        await _adapter.StopDiscoveryAsync();
    }
}
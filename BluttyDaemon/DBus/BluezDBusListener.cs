using BluttyDaemon.Exceptions;
using HashtagChris.DotNetBlueZ;
using Microsoft.Extensions.Logging;

namespace BluttyDaemon;

public class BluezDBusListener : IBluezListener
{
    public static async Task<BluezDBusListener> PrepareDefaultAsync(ILogger<BluezDBusListener> logger)
    {
        var accessibleAdapters = await BlueZManager.GetAdaptersAsync();
        return accessibleAdapters.Count == 0
            ? throw new NotAnyDeviceAccessibleException()
            : new BluezDBusListener(accessibleAdapters[0], logger);
    }

    private readonly Adapter _adapter;
    private readonly ILogger<BluezDBusListener> _logger;

    public BluezDBusListener(IAdapter1 adapter, ILogger<BluezDBusListener> logger)
    {
        _adapter = (Adapter)adapter;
        _logger = logger;
    }

    public async Task StartDiscoveryAsync(DeviceChangeEventHandlerAsync deviceFoundHandler)
    {
        var adapterAttrs = await _adapter.GetAllAsync();
        _logger.LogDiscoveryStarted(adapterAttrs.Address, adapterAttrs.Name);
        _adapter.DeviceFound += deviceFoundHandler;
        await _adapter.StartDiscoveryAsync();
    }

    public async Task StopDiscoveryAsync()
    {
        await _adapter.StopDiscoveryAsync();
        var adapterAttrs = await _adapter.GetAllAsync();
        _logger.LogDiscoveryStopped(adapterAttrs.Address, adapterAttrs.Name);
    }
}
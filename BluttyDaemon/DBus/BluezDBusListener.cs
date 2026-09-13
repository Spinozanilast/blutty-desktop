using Bluezat;
using Bluezat.Events;
using Bluezat.Wrappers;
using Microsoft.Extensions.Logging;
using Tmds.DBus.Protocol;

namespace BluttyDaemon;

public class BluezDBusListener : IBluezListener
{
    public static async Task<BluezDBusListener> PrepareDefaultAsync(DBusConnection connection,
        ILogger<BluezDBusListener> logger)
    {
        var defaultAdapter = await BluezManager.GetDefaultAdapterAsync(connection);
        return new BluezDBusListener(defaultAdapter, logger);
    }

    private readonly Adapter _adapter;
    private readonly ILogger<BluezDBusListener> _logger;

    public BluezDBusListener(Adapter adapter, ILogger<BluezDBusListener> logger)
    {
        _adapter = adapter;
        _logger = logger;
    }

    public async Task StartDiscoveryAsync(DeviceChangeEventHandlerAsync deviceFoundHandler)
    {
        var adapterAttrs = await _adapter.GetPropertiesAsync();
        _logger.LogDiscoveryStarted(adapterAttrs.Address, adapterAttrs.Name);
        _adapter.DeviceFound += deviceFoundHandler;
        await _adapter.StartDiscoveryAsync();
    }

    public async Task StopDiscoveryAsync()
    {
        await _adapter.StopDiscoveryAsync();
        var adapterAttrs = await _adapter.GetPropertiesAsync();
        _logger.LogDiscoveryStopped(adapterAttrs.Address, adapterAttrs.Name);
    }
}
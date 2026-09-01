using System.IO.Pipes;
using BluttyRpc;
using StreamJsonRpc;

namespace BluttyDaemon;

internal class BluttyDaemon
{
    private Notifier _notifier;

    private readonly BluezDBusListener _bluezListener;
    private readonly IDeviceEventsProcessor _deviceEventsProcessor;

    public BluttyDaemon(BluezDBusListener bluezListener, IDeviceEventsProcessor deviceEventsProcessor)
    {
        _bluezListener = bluezListener;
        _deviceEventsProcessor = deviceEventsProcessor;
    }

    public async Task StartDaemonAsync()
    {
        await _bluezListener.StartDiscoveryAsync(_deviceEventsProcessor.OnDeviceFoundAsync);
        await Task.Delay(-1);
    }

    public async Task StopDaemonAsync()
    {
        await _bluezListener.StopDiscoveryAsync();
    }
}
using Microsoft.Extensions.Logging;

namespace BluttyDaemon;

internal class BluttyDaemon
{
    private readonly ILogger<BluttyDaemon> _logger;
    private readonly BluezDBusListener _bluezListener;
    private readonly IDeviceEventsProcessor _deviceEventsProcessor;

    public BluttyDaemon(BluezDBusListener bluezListener, IDeviceEventsProcessor deviceEventsProcessor,
        ILogger<BluttyDaemon> logger)
    {
        _bluezListener = bluezListener;
        _deviceEventsProcessor = deviceEventsProcessor;
        _logger = logger;
    }

    public async Task StartDaemonAsync()
    {
        var hostName = Environment.MachineName;
        _logger.LogDaemonStarting(hostName);

        await _bluezListener.StartDiscoveryAsync(_deviceEventsProcessor.OnDeviceFoundAsync);

        _logger.LogDaemonStarted(hostName);
        await Task.Delay(-1);
    }

    public async Task StopDaemonAsync()
    {
        _logger.LogDaemonStopping();
        await _bluezListener.StopDiscoveryAsync();
        _logger.LogDaemonStopped();
    }
}
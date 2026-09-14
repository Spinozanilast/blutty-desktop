using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Bluezat.DBus;
using Bluezat.Events;
using BluttyRpc;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Adapter = Bluezat.Wrappers.Adapter;
using Device = Bluezat.Wrappers.Device;

namespace BluttyDaemon;

public class DeviceEventsProcessor : IDeviceEventsProcessor, IDisposable
{
    private const int BatteryPollAttempts = 20;
    private static readonly TimeSpan BatteryPollInterval = TimeSpan.FromMilliseconds(500);

    private readonly IDeviceConnector _deviceConnector;
    private readonly ILogger<DeviceEventsProcessor> _logger;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _batteryPollers = new();

    /// <summary>
    /// Initialize processor with NamedPipeClientStream disposable instance 
    /// </summary>
    public DeviceEventsProcessor(NamedPipeClientStream readyClientStream, ILogger<DeviceEventsProcessor> logger)
    {
        _deviceConnector = JsonRpc.Attach<IDeviceConnector>(readyClientStream);
        _logger = logger;
    }

    public void Dispose()
    {
        foreach (var poller in _batteryPollers.Values)
        {
            poller.Cancel();
            poller.Dispose();
        }

        _batteryPollers.Clear();
        _deviceConnector?.Dispose();
    }

    public async Task OnDeviceFoundAsync(Adapter adapter, FoundDeviceBluezEventArgs args)
    {
        try
        {
            var device = args.Device;
            var deviceAttrs = await device.GetPropertiesAsync();
            _logger.LogDeviceFound(deviceAttrs.Address, deviceAttrs.Name);

            device.Connected += OnDeviceConnectedAsync;
            device.Disconnected += OnDeviceDisconnectedAsync;
        }
        catch (Exception exception)
        {
            _logger.LogDeviceSubscribeFailed(exception);
        }
    }

    public async Task OnDeviceConnectedAsync(Device device, BluezEventArgs args)
    {
        try
        {
            var deviceAttrs = await device.GetPropertiesAsync();

            _logger.LogDeviceConnected(deviceAttrs.Address, deviceAttrs.Name);

            _deviceConnector.SendConnectedDeviceInfo(isConnected: true, ToDeviceInfo(deviceAttrs));
            StartBatteryPolling(device, deviceAttrs.Address);
        }
        catch (Exception exception)
        {
            _logger.LogDeviceNotifyFailed(exception);
        }
    }

    private void StartBatteryPolling(Device device, string address)
    {
        var cts = _batteryPollers.AddOrUpdate(
            address,
            _ => new CancellationTokenSource(),
            (_, existing) =>
            {
                existing.Cancel();
                existing.Dispose();
                return new CancellationTokenSource();
            });

        _ = Task.Run(() => PollBatteryPercentageAsync(device, address, cts.Token));
    }

    private void StopBatteryPolling(string address)
    {
        if (_batteryPollers.TryRemove(address, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    private async Task PollBatteryPercentageAsync(Device device, string address, CancellationToken token)
    {
        for (var attempt = 0; attempt < BatteryPollAttempts && !token.IsCancellationRequested; attempt++)
        {
            try
            {
                var batteryPercentage = await device.GetBatteryPercentageAsync();
                if (batteryPercentage > 0)
                {
                    _deviceConnector.SendBatteryPercentage(address, batteryPercentage);
                    return;
                }
            }
            catch
            {
                // Battery1 interface may not be resolved yet; keep retrying.
            }

            try
            {
                await Task.Delay(BatteryPollInterval, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    public async Task OnDeviceDisconnectedAsync(Device device, BluezEventArgs args)
    {
        try
        {
            var deviceAttrs = await device.GetPropertiesAsync();
            _logger.LogDeviceDisconnected(deviceAttrs.Address, deviceAttrs.Name);

            StopBatteryPolling(deviceAttrs.Address);

            _deviceConnector.SendConnectedDeviceInfo(isConnected: false, ToDeviceInfo(deviceAttrs));
        }
        catch (Exception exception)
        {
            _logger.LogDeviceNotifyFailed(exception);
        }
    }

    private static DeviceInfo ToDeviceInfo(Bluezat.DBus.DeviceProperties properties) => new(
        properties.Address,
        properties.Name,
        properties.Alias,
        properties.Paired,
        properties.Icon
    );
}
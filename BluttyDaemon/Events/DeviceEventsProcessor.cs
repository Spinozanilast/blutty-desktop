using System.IO.Pipes;
using BluttyRpc;
using HashtagChris.DotNetBlueZ;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;

namespace BluttyDaemon;

public class DeviceEventsProcessor : IDeviceEventsProcessor, IDisposable
{
    private readonly IDeviceConnector _deviceConnector;
    private readonly ILogger<DeviceEventsProcessor> _logger;

    /// <summary>
    /// Initialize processor with NamedPipeClientStream disposable instance 
    /// </summary>
    /// <param name="readyClientStream"></param>
    public DeviceEventsProcessor(NamedPipeClientStream readyClientStream, ILogger<DeviceEventsProcessor> logger)
    {
        _deviceConnector = JsonRpc.Attach<IDeviceConnector>(readyClientStream);
        _logger = logger;
    }

    public void Dispose() => _deviceConnector?.Dispose();

    public async Task OnDeviceFoundAsync(Adapter adapter, DeviceFoundEventArgs args)
    {
        try
        {
            var device = args.Device;
            var deviceAttrs = await device.GetAllAsync();
            _logger.LogDeviceFound(deviceAttrs.Address, deviceAttrs.Name);

            device.Connected += OnDeviceConnectedAsync;
            device.Disconnected += OnDeviceDisconnectedAsync;
        }
        catch (Exception exception)
        {
            _logger.LogDeviceSubscribeFailed(exception);
        }
    }

    public async Task OnDeviceConnectedAsync(Device device, BlueZEventArgs args)
    {
        try
        {
            var deviceAttrs = await device.GetAllAsync();
            _logger.LogDeviceConnected(deviceAttrs.Address, deviceAttrs.Name);

            _deviceConnector.SendConnectedDeviceInfo(isConnected: true, deviceAttrs);
        }
        catch (Exception exception)
        {
            _logger.LogDeviceNotifyFailed(exception);
        }
    }

    public async Task OnDeviceDisconnectedAsync(Device device, BlueZEventArgs args)
    {
        try
        {
            var deviceAttrs = await device.GetAllAsync();
            _logger.LogDeviceDisconnected(deviceAttrs.Address, deviceAttrs.Name);

            _deviceConnector.SendConnectedDeviceInfo(isConnected: false, deviceAttrs);
        }
        catch (Exception exception)
        {
            _logger.LogDeviceNotifyFailed(exception);
        }
    }
}
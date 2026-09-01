using System.IO.Pipes;
using BluttyRpc;
using HashtagChris.DotNetBlueZ;
using StreamJsonRpc;

namespace BluttyDaemon;

public class DeviceEventsProcessor : IDeviceEventsProcessor
{
    private readonly NamedPipeClientStream _readyClientStream;

    /// <summary>
    /// Initialize processor with NamedPipeClientStream disposable instance 
    /// </summary>
    /// <param name="readyClientStream"></param>
    public DeviceEventsProcessor(NamedPipeClientStream readyClientStream)
    {
        _readyClientStream = readyClientStream;
    }

    public Task OnDeviceFoundAsync(Adapter adapter, DeviceFoundEventArgs args)
    {
        try
        {
            var device = args.Device;

            device.Connected += OnDeviceConnectedAsync;
            device.Disconnected += OnDeviceDisconnectedAsync;
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    public async Task OnDeviceConnectedAsync(Device device, BlueZEventArgs args)
    {
        var deviceAttrs = await device.GetAllAsync();

        using var deviceConnector = JsonRpc.Attach<IDeviceConnector>(stream: _readyClientStream);
        deviceConnector.SendConnectedDeviceInfo(isConnected: true, deviceAttrs);
    }

    public async Task OnDeviceDisconnectedAsync(Device device, BlueZEventArgs args)
    {
        var deviceAttrs = await device.GetAllAsync();

        using var deviceConnector = JsonRpc.Attach<IDeviceConnector>(stream: _readyClientStream);
        deviceConnector.SendConnectedDeviceInfo(isConnected: false, deviceAttrs);
    }
}
using HashtagChris.DotNetBlueZ;

namespace BluttyDaemon;

public interface IDeviceEventsProcessor
{
    Task OnDeviceFoundAsync(Adapter adapter, DeviceFoundEventArgs eventArgs);
    Task OnDeviceConnectedAsync(Device device, BlueZEventArgs eventArgs);
    Task OnDeviceDisconnectedAsync(Device device, BlueZEventArgs eventArgs);
}

using Bluezat.Events;
using Bluezat.Wrappers;

namespace BluttyDaemon;

public interface IDeviceEventsProcessor
{
    Task OnDeviceFoundAsync(Adapter adapter, FoundDeviceBluezEventArgs eventArgs);
    Task OnDeviceConnectedAsync(Device device, BluezEventArgs eventArgs);
    Task OnDeviceDisconnectedAsync(Device device, BluezEventArgs eventArgs);
}
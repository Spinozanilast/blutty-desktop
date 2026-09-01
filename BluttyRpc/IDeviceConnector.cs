using HashtagChris.DotNetBlueZ;
using DeviceProperties = HashtagChris.DotNetBlueZ.Device1Properties;

namespace BluttyRpc;

public interface IDeviceConnector : IDisposable
{
    void SendConnectedDeviceInfo(bool isConnected, DeviceProperties info);
    void ConnectToDevice(string address);
    void DisconnectFromDevice(string address);
}
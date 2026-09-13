using Bluezat.DBus;

namespace BluttyRpc;

public interface IDeviceConnector : IDisposable
{
    void SendConnectedDeviceInfo(bool isConnected, DeviceProperties info);
    void ConnectToDevice(string address);
    void DisconnectFromDevice(string address);
}
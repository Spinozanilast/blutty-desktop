namespace BluttyRpc;

public interface IDeviceConnector : IDisposable
{
    void SendConnectedDeviceInfo(bool isConnected, DeviceInfo info);
    void SendBatteryPercentage(string address, byte batteryPercentage);
    void ConnectToDevice(string address);
    void DisconnectFromDevice(string address);
}

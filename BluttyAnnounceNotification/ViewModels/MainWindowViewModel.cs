using System;
using Blutty.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Blutty.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string DeviceName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string DeviceAddress { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool IsConnected { get; set; }
    [ObservableProperty]
    public partial string StatusText { get; set; } = string.Empty;
    [ObservableProperty]
    public partial BluetoothConnectionState BluetoothState { get; set; } = BluetoothConnectionState.Connected;

    public event Action? NotificationRequested;

    public void OnDeviceEvent(bool isConnected, string name, string address)
    {
        DeviceName = name;
        DeviceAddress = address;
        IsConnected = isConnected;
        StatusText = isConnected ? "Connected" : "Disconnected";
        BluetoothState = isConnected ? BluetoothConnectionState.Connected : BluetoothConnectionState.Disconnected;
        NotificationRequested?.Invoke();
    }
}

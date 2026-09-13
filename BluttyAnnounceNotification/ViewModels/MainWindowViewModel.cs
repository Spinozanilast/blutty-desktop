using System;
using Bluezat.DBus;
using Blutty.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Blutty.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial string DeviceName { get; set; } = string.Empty;
    [ObservableProperty] public partial string DeviceAddress { get; set; } = string.Empty;
    [ObservableProperty] public partial bool IsConnected { get; set; }
    [ObservableProperty] public partial string StatusText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial BluetoothConnectionState BluetoothState { get; set; } = BluetoothConnectionState.Connected;

    public event Action? NotificationRequested;

    public void OnDeviceEvent(bool isConnected, DeviceProperties deviceProps)
    {
        DeviceName = deviceProps.Alias.Length == 0 ? deviceProps.Name : deviceProps.Alias;
        DeviceAddress = deviceProps.Address;
        IsConnected = isConnected;
        StatusText = isConnected ? "Connected" : "Disconnected";
        BluetoothState = isConnected ? BluetoothConnectionState.Connected : BluetoothConnectionState.Disconnected;
        NotificationRequested?.Invoke();
    }
}
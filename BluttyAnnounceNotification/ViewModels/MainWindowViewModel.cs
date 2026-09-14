using System;
using Blutty.Controls;
using Blutty.Services;
using BluttyRpc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Blutty.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial string DeviceName { get; set; } = string.Empty;
    [ObservableProperty] public partial string DeviceAddress { get; set; } = string.Empty;
    [ObservableProperty] public partial bool IsConnected { get; set; }
    [ObservableProperty] public partial string StatusText { get; set; } = string.Empty;
    [ObservableProperty] public partial double ProgressValue { get; set; }
    [ObservableProperty] public partial bool IsTextVisible { get; set; } = true;

    [ObservableProperty]
    public partial BluetoothConnectionState BluetoothState { get; set; } = BluetoothConnectionState.Connected;

    public event Action? NotificationRequested;

    public MainWindowViewModel(DeviceConnectorService deviceConnectorService)
    {
        deviceConnectorService.DeviceChanged += OnDeviceEvent;
        deviceConnectorService.BatteryChanged += OnBatteryChanged;
    }

    public void OnDeviceEvent(bool isConnected, DeviceInfo deviceInfo)
    {
        DeviceName = deviceInfo.Alias.Length == 0 ? deviceInfo.Name : deviceInfo.Alias;
        DeviceAddress = deviceInfo.Address;
        IsConnected = isConnected;
        StatusText = isConnected ? "Connected" : "Disconnected";
        ProgressValue = 0;
        IsTextVisible = false;
        BluetoothState = isConnected ? BluetoothConnectionState.Connected : BluetoothConnectionState.Disconnected;
        NotificationRequested?.Invoke();
    }

    private void OnBatteryChanged(string address, byte batteryPercentage)
    {
        if (!string.Equals(address, DeviceAddress, StringComparison.OrdinalIgnoreCase)) return;

        ProgressValue = batteryPercentage;
        IsTextVisible = batteryPercentage > 0;
    }
}
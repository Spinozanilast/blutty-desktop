using Avalonia;
using Lucide.Avalonia;

namespace Blutty.Controls;

public enum BluetoothConnectionState
{
    Connected,
    Disconnected,
}

public class BluetoothStateIcon : LucideIcon
{
    public static readonly StyledProperty<BluetoothConnectionState> StateProperty =
        AvaloniaProperty.Register<BluetoothStateIcon, BluetoothConnectionState>(nameof(State), BluetoothConnectionState.Connected);

    public BluetoothConnectionState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public BluetoothStateIcon()
    {
        StateProperty.Changed.AddClassHandler<BluetoothStateIcon>((icon, _) => icon.UpdateKind());
        UpdateKind();
    }

    private void UpdateKind()
    {
        Kind = State == BluetoothConnectionState.Connected
            ? LucideIconKind.BluetoothConnected
            : LucideIconKind.BluetoothOff;
    }
}
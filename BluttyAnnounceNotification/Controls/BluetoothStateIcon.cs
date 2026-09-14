using Avalonia;
using Avalonia.Media;
using Lucide.Avalonia;
using Color = System.Drawing.Color;

namespace Blutty.Controls;

public enum BluetoothConnectionState
{
    Connected,
    Disconnected,
}

public class BluetoothStateIcon : LucideIcon
{
    public static readonly StyledProperty<BluetoothConnectionState> StateProperty =
        AvaloniaProperty.Register<BluetoothStateIcon, BluetoothConnectionState>(nameof(State),
            BluetoothConnectionState.Connected);

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
        if (State == BluetoothConnectionState.Connected)
        {
            Kind = LucideIconKind.BluetoothConnected;
            Foreground = new SolidColorBrush(Colors.LimeGreen);
            return;
        }

        Kind = LucideIconKind.BluetoothOff;
        Foreground = new SolidColorBrush(Colors.DarkRed);
    }
}
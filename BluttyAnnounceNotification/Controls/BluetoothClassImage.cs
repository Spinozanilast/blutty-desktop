// using Avalonia;
// using Avalonia.Controls;
// using Lucide.Avalonia;
//
// namespace Blutty.Controls;
//
// public class BluetoothClassImage : Image
// {
//     public static readonly StyledProperty<BluetoothConnectionState> ClassTypeProperty =
//         AvaloniaProperty.Register<BluetoothStateIcon, BluetoothConnectionState>(nameof(State), BluetoothConnectionState.Connected);
//
//     public BluetoothConnectionState ClassType
//     {
//         get => GetValue(ClassTypeProperty);
//         set => SetValue(ClassTypeProperty, value);
//     }
//
//     public BluetoothClassImage()
//     {
//         ClassTypeProperty.Changed.AddClassHandler<BluetoothStateIcon>((icon, _) => icon.UpdateKind());
//         UpdateKind();
//     }
// }
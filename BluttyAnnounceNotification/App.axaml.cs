using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Blutty.Services;
using Blutty.ViewModels;
using Blutty.Views;

namespace Blutty;

public partial class App : Application
{
    private DeviceConnectorService? _deviceConnectorService;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = new MainWindowViewModel();
            _deviceConnectorService = new DeviceConnectorService(viewModel.OnDeviceEvent);
            viewModel.NotificationRequested += () =>
            {
                if (desktop.MainWindow is MainWindow window)
                {
                    window.HandleNotification();
                }
            };

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            desktop.Exit += OnDesktopExit;

            _ = Task.Run(() => _deviceConnectorService.StartAsync(default));
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _deviceConnectorService?.Dispose();
    }
}

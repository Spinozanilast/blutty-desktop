using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Blutty.Services;
using Blutty.ViewModels;
using Blutty.Views;
using BluttyRpc;
using Microsoft.Extensions.DependencyInjection;

namespace Blutty;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _serviceProvider = BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var window = _serviceProvider.GetRequiredService<MainWindow>();
            window.DataContext = viewModel;

            viewModel.NotificationRequested += window.HandleNotification;

            // Do not assign desktop.MainWindow: the classic lifetime force-shows it, which
            // races the Wayland size negotiation. The window is shown on demand instead.
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            desktop.Exit += OnDesktopExit;

            var deviceConnectorService = _serviceProvider.GetRequiredService<DeviceConnectorService>();
            _ = Task.Run(() => deviceConnectorService.StartAsync(CancellationToken.None));
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton(_ =>
        {
            var settings = new SettingsService();
            settings.Load();
            return settings;
        });

        services.AddSingleton<DeviceConnectorService>();
        services.AddSingleton<IDeviceConnector>(sp => sp.GetRequiredService<DeviceConnectorService>());
        services.AddSingleton<HyprlandCommandService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<SettingsViewModel>();

        services.AddSingleton<MainWindow>();
        services.AddTransient<Settings>();

        return services.BuildServiceProvider();
    }

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _serviceProvider?.Dispose();
    }
}
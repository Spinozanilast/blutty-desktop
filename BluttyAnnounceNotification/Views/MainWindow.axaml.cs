using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Blutty.Services;
using Blutty.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using PlatformsPipes.Commands;
using PlatformsPipes.Helpers;
using SukiUI.Controls;

namespace Blutty.Views;

public partial class MainWindow : SukiWindow
{
    private readonly SettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;
    private DispatcherTimer? _hideTimer;

    private readonly HyprlandCommandService _hyprlandCommandService;
    private bool _hyprlandWindowReady;

    public MainWindow(SettingsService settingsService, IServiceProvider serviceProvider,
        HyprlandCommandService hyprlandCommandService)
    {
        InitializeComponent();
        _settingsService = settingsService;
        _serviceProvider = serviceProvider;
        _hyprlandCommandService = hyprlandCommandService;

        SettingsButton.Click += OnSettingsClick;
    }

    private async Task PositionOnConfiguredScreenAsync()
    {
        if (Screens is not { } screens) return;

        if (OperatingSystem.UsesWayland())
        {
            await WaitForScreensAsync(screens);
        }

        if (OperatingSystem.UsesHyprlandComposer() && !_hyprlandWindowReady)
        {
            await Task.Delay(250);
            _hyprlandWindowReady = true;
        }

        if (screens.All.Count == 0) return;

        MoveNotificationWindow(offScreen: false);
    }
    
    private void MoveNotificationWindow(bool offScreen)
    {
        if (Screens is not { } screens || screens.All.Count == 0) return;

        var allScreens = screens.All;
        var screenIndex = _settingsService.Settings.NotificationScreenIndex;
        var screen = (screenIndex >= 1 && screenIndex < allScreens.Count)
            ? allScreens[screenIndex]
            : screens.ScreenFromWindow(this) ?? allScreens[0];

        var workArea = screen.WorkingArea;

        var x = workArea.X + (workArea.Width - Width) / 2;
        var y = offScreen
            ? workArea.Y + workArea.Height
            : workArea.Y + workArea.Height - Height - 16;

        if (OperatingSystem.UsesHyprlandComposer())
        {
            var moveWindowCmd = new Hyprlua.MoveToPxCommand(
                x: (int)x,
                y: (int)y,
                relative: false,
                windowDefinition: Hyprland.CreateWinDefinition(HyprlandWinDefiner.Title,
                    $"^{Title}$"));

            _hyprlandCommandService.Dispatch(moveWindowCmd);
        }
        else
        {
            Position = new PixelPoint((int)x, (int)y);
        }
    }

    private static async Task WaitForScreensAsync(Screens screens)
    {
        if (screens.All.Count > 0) return;

        var screensLoaded = new TaskCompletionSource();

        screens.Changed += OnScreensChanged;
        try
        {
            await screensLoaded.Task;
        }
        finally
        {
            screens.Changed -= OnScreensChanged;
        }

        return;

        void OnScreensChanged(object? sender, EventArgs e)
        {
            if (screens.All.Count > 0)
            {
                screensLoaded.TrySetResult();
            }
        }
    }

    public void HandleNotification()
    {
        Dispatcher.UIThread.Post(async () =>
        {
            if (!OperatingSystem.UsesWayland())
            {
                await PositionOnConfiguredScreenAsync();
            }

            Show();
            Topmost = true;

            if (OperatingSystem.UsesWayland())
            {
                await PositionOnConfiguredScreenAsync();
            }

            _hideTimer?.Stop();
            _hideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(_settingsService.Settings.NotificationDelaySeconds)
            };
            _hideTimer.Tick += (_, _) =>
            {
                _hideTimer.Stop();
                HideNotification();
            };
            _hideTimer.Start();
        });
    }

    private void HideNotification()
    {
        if (OperatingSystem.UsesHyprlandComposer())
        {
            MoveNotificationWindow(offScreen: true);
        }
        else
        {
            Hide();
        }
    }

    private void OnSettingsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = _serviceProvider.GetRequiredService<SettingsViewModel>();
        vm.SetScreens(Screens.All);

        var settingsWindow = _serviceProvider.GetRequiredService<Settings>();
        settingsWindow.DataContext = vm;

        settingsWindow.Show();
    }
}
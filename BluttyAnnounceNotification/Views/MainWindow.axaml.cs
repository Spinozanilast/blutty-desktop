using System;
using Avalonia;
using Avalonia.Threading;
using Blutty.Services;
using Blutty.ViewModels;
using SukiUI.Controls;

namespace Blutty.Views;

public partial class MainWindow : SukiWindow
{
    private readonly SettingsService _settingsService;
    private DispatcherTimer? _hideTimer;
    private bool _isInitialized;

    public MainWindow()
    {
        InitializeComponent();
        _settingsService = new SettingsService();
        _settingsService.Load();
        Opened += OnOpened;

 

        SettingsButton.Click += OnSettingsClick;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (_isInitialized) return;
        _isInitialized = true;

        PositionOnConfiguredScreen();
        Hide();
    }

    private void PositionOnConfiguredScreen()
    {
        var screens = Screens.All;
        if (screens.Count == 0) return;

        var screenIndex = _settingsService.Settings.NotificationScreenIndex;
        var screen = (screenIndex >= 1 && screenIndex <= screens.Count)
            ? screens[screenIndex]
            : Screens.ScreenFromWindow(this) ?? screens[0];

        var workArea = screen.WorkingArea;
        var x = workArea.X + (workArea.Width - Width) / 2;
        var y = workArea.Y + workArea.Height - Height - 16;
        Position = new PixelPoint((int)x, (int)y);
    }

    public void HandleNotification()
    {
        Dispatcher.UIThread.Post(() =>
        {
            PositionOnConfiguredScreen();
            Show();
            Topmost = true;

            _hideTimer?.Stop();
            _hideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(_settingsService.Settings.NotificationDelaySeconds)
            };
            _hideTimer.Tick += (_, _) =>
            {
                _hideTimer.Stop();
                Hide();
            };
            _hideTimer.Start();
        });
    }

    private void OnSettingsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = new SettingsViewModel(_settingsService);
        vm.SetScreens(Screens.All);
        
        var settingsWindow = new Settings(vm);

        var screen = Screens.ScreenFromWindow(this);
        if (screen is not null)
        {
            var workArea = screen.WorkingArea;
            var x = workArea.X + (workArea.Width - settingsWindow.Width) / 2;
            var y = workArea.Y + (workArea.Height - settingsWindow.Height) / 2;
            settingsWindow.Position = new PixelPoint((int)x, (int)y);
        }

        settingsWindow.ShowDialog(this);
    }
}

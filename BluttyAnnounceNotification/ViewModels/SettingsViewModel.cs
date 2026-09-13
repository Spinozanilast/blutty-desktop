using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blutty.ViewModels;

public class ScreenInfo
{
    public int Index { get; set; }
    public string DisplayName { get; set; } = "";
    public override string ToString() => DisplayName;
}

public partial class SettingsViewModel : ViewModelBase
{
    private readonly Services.SettingsService _settingsService;

    public ObservableCollection<ScreenInfo> AvailableScreens { get; } = [];

    [ObservableProperty] private int _selectedScreenIndex = 0;

    [ObservableProperty] private double _notificationDelaySeconds = 4.0;

    public SettingsViewModel(Services.SettingsService settingsService)
    {
        _settingsService = settingsService;
        LoadFromSettings();
    }

    public void SetScreens(IReadOnlyList<Screen> screens)
    {
        AvailableScreens.Clear();

        AvailableScreens.Add(new ScreenInfo
        {
            Index = 0,
            DisplayName = "Active"
        });

        for (var i = 0; i < screens.Count; i++)
        {
            var s = screens[i];
            AvailableScreens.Add(new ScreenInfo
            {
                Index = i + 1,
                DisplayName =
                    $"{s.DisplayName} ({s.Bounds.Width}x{s.Bounds.Height}) - {s.WorkingArea.X},{s.WorkingArea.Y}"
            });
        }
    }

    private void LoadFromSettings()
    {
        var s = _settingsService.Settings;
        SelectedScreenIndex = s.NotificationScreenIndex;
        NotificationDelaySeconds = s.NotificationDelaySeconds;
    }

    [RelayCommand]
    private void Save()
    {
        _settingsService.Settings.NotificationScreenIndex = SelectedScreenIndex;
        _settingsService.Settings.NotificationDelaySeconds = NotificationDelaySeconds;
        _settingsService.Save();
    }
}
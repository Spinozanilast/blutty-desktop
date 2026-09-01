using Avalonia.Platform;
using Blutty.ViewModels;
using SukiUI.Controls;

namespace Blutty.Views;

public partial class Settings : SukiWindow
{
    public Settings(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

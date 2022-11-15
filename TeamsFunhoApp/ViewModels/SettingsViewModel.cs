using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;

using TeamsFunhoApp.Contracts.Services;
using TeamsFunhoApp.Helpers;

using Windows.ApplicationModel;

namespace TeamsFunhoApp.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IAppSettingsService _appSettingsService;
    [ObservableProperty]
    private ElementTheme _elementTheme;
    [ObservableProperty]
    private string _versionDescription;
    [ObservableProperty]
    private string _incomingWebhookUrl;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IAppSettingsService appSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _appSettingsService = appSettingsService;
        _elementTheme = _themeSelectorService.Theme;
        _incomingWebhookUrl = _appSettingsService.IncomingWebhookUrl;
        _versionDescription = GetVersionDescription();
    }

    [RelayCommand]
    private async Task UpdateIncomingWebhookUrl()
    {
        await _appSettingsService.SetIncomingWebhookUrlAsync(IncomingWebhookUrl);
    }

    [RelayCommand]
    private async Task SwitchTheme(ElementTheme elementTheme)
    {
        if (ElementTheme != elementTheme)
        {
            ElementTheme = elementTheme;
            await _themeSelectorService.SetThemeAsync(elementTheme);
        }
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    partial void OnIncomingWebhookUrlChanged(string value)
    {
        UpdateIncomingWebhookUrlCommand.Execute(null);
    }
}

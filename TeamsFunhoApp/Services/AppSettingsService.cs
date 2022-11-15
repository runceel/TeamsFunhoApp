using CommunityToolkit.Mvvm.ComponentModel;
using TeamsFunhoApp.Contracts.Services;
using TeamsFunhoApp.Models;

namespace TeamsFunhoApp.Services;
public class AppSettingsService : IAppSettingsService
{
    private const int DefaultWindowWidth = 300;
    private const int DefaultWindowHeight = 400;

    private readonly string IncomingWebhookUrlSettingsKey = $"{nameof(AppSettingsService)}_{nameof(IncomingWebhookUrl)}";
    private readonly string WindowSizeSettingsKey = $"{nameof(AppSettingsService)}_WindowSize";
    private readonly ILocalSettingsService localSettingsService;

    public string IncomingWebhookUrl { get; private set; } = "";

    private WindowSize? _windowSize;
    public int WindowWidth => _windowSize?.Width ?? DefaultWindowWidth;
    public int WindowHeight => _windowSize?.Height ?? DefaultWindowHeight;

    public AppSettingsService(ILocalSettingsService localSettingsService)
    {
        this.localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        IncomingWebhookUrl = await localSettingsService.ReadSettingAsync<string>(IncomingWebhookUrlSettingsKey) ?? "";
        _windowSize = await localSettingsService.ReadSettingAsync<WindowSize>(WindowSizeSettingsKey);
    }

    public async Task SetIncomingWebhookUrlAsync(string incomingWebhookUrl)
    {
        if (IncomingWebhookUrl != incomingWebhookUrl)
        {
            IncomingWebhookUrl = incomingWebhookUrl;
            await localSettingsService.SaveSettingAsync(IncomingWebhookUrlSettingsKey, incomingWebhookUrl);
        }
    }

    public async Task SetWindowSizeAsync(int windowWidth, int windowHeight)
    {
        if (WindowWidth == windowWidth && WindowHeight == windowHeight) return; 

        _windowSize = new() { Width = windowWidth, Height = windowHeight };
        await localSettingsService.SaveSettingAsync(WindowSizeSettingsKey, _windowSize);
    }
}

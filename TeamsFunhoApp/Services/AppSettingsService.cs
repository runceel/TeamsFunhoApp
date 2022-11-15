using CommunityToolkit.Mvvm.ComponentModel;
using TeamsFunhoApp.Contracts.Services;

namespace TeamsFunhoApp.Services;
public class AppSettingsService : IAppSettingsService
{
    private readonly string IncomingWebhookUrlSettingsKey = $"{nameof(AppSettingsService)}_{nameof(IncomingWebhookUrl)}";
    private readonly ILocalSettingsService localSettingsService;

    public string IncomingWebhookUrl
    {
        get;
        private set;
    } = "";

    public AppSettingsService(ILocalSettingsService localSettingsService)
    {
        this.localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync() =>
        IncomingWebhookUrl = await localSettingsService.ReadSettingAsync<string>(IncomingWebhookUrlSettingsKey) ?? "";
    public async Task SetIncomingWebhookUrl(string incomingWebhookUrl)
    {
        if (IncomingWebhookUrl != incomingWebhookUrl)
        {
            IncomingWebhookUrl = incomingWebhookUrl;
            await localSettingsService.SaveSettingAsync(IncomingWebhookUrlSettingsKey, incomingWebhookUrl);
        }
    }
}

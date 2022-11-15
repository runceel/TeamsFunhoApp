namespace TeamsFunhoApp.Contracts.Services;
public interface IAppSettingsService
{
    public string IncomingWebhookUrl
    {
        get;
    }

    Task InitializeAsync();
    Task SetIncomingWebhookUrl(string incomingWebhookUrl);
}

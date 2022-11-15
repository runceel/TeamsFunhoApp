namespace TeamsFunhoApp.Contracts.Services;
public interface IAppSettingsService
{
    public string IncomingWebhookUrl { get; }

    public int WindowWidth { get; }
    public int WindowHeight { get; }
    Task InitializeAsync();
    Task SetIncomingWebhookUrlAsync(string incomingWebhookUrl);
    Task SetWindowSizeAsync(int windowWidth, int windowHeight);
}

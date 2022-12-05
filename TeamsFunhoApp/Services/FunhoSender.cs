using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Text;
using TeamsFunhoApp.Contracts.Services;

namespace TeamsFunhoApp.Services;
public class FunhoSender : IFunhoSender
{
    private static JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAppSettingsService _appSettingsService;

    public FunhoSender(IHttpClientFactory httpClientFactory, IAppSettingsService appSettingsService)
    {
        _httpClientFactory = httpClientFactory;
        _appSettingsService = appSettingsService;
    }

    public async Task<(bool Success, string? Reason)> SendAsync(string message)
    {
        if (!Uri.TryCreate(_appSettingsService.IncomingWebhookUrl, UriKind.Absolute, out var _))
        {
            return (false, "IncomingWebhook URL を設定画面で構成してください。");
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            return (false, "メッセージが空です。");
        }

        string json = CreateJson(message);
        var request = new HttpRequestMessage(HttpMethod.Post, _appSettingsService.IncomingWebhookUrl)
        {
            Content = new StringContent(json,
                Encoding.UTF8,
                "application/json"),
        };
        try
        {
            var response = await _httpClientFactory.CreateClient().SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return (false, $"エラーコード: {response.StatusCode}");
            }

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            return (false, $"エラーメッセージ: {ex.Message}({ex.StatusCode?.ToString() ?? "None"})");
        }
    }

    private static string CreateJson(string message)
    {
        var normalizedMessage = message.ReplaceLineEndings();
        var paragraphs = normalizedMessage
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(x => new AdaptiveTextBlock(x));
        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0));
        card.Body.AddRange(paragraphs);

        var attachment = new Attachment
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = card,
        };

        var activity = Activity.CreateMessageActivity();
        activity.Attachments = new List<Attachment> { attachment };
        return JsonConvert.SerializeObject(activity, JsonSerializerSettings);
    }
}

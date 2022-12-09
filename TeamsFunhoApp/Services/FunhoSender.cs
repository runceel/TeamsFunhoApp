using Accessibility;
using AdaptiveCards;
using AngleSharp;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Security.Policy;
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

        string json = await CreateIncomingWebhookJsonAsync(message);
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

    private async Task<string> CreateIncomingWebhookJsonAsync(string message)
    {
        var normalizedMessage = message.ReplaceLineEndings();
        var paragraphTasks = normalizedMessage
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(x => x.Trim())
            .Select(x => ProcessTextForMarkdownAsync(x));
        var paragraphs = (await Task.WhenAll(paragraphTasks))
            .Select(x => new AdaptiveTextBlock(x) { Wrap = true });
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
    private async Task<string> ProcessTextForMarkdownAsync(string x)
    {
        if (!Uri.TryCreate(x, UriKind.Absolute, out var _)) return x;

        var label = x;
        try
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(x);
            var cellSelector = "title";
            var cells = document.QuerySelectorAll(cellSelector);
            label = cells.FirstOrDefault()?.TextContent ?? label;
        }
        catch 
        {
        }

        return $"[{label}]({x})";
    }
}

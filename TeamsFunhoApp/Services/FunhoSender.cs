using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamsFunhoApp.Contracts.Services;

namespace TeamsFunhoApp.Services;
public class FunhoSender : IFunhoSender
{
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

        var response = await _httpClientFactory.CreateClient().PostAsync(
            _appSettingsService.IncomingWebhookUrl,
            new StringContent($$"""
                {
                    "text": "{{message}}"
                }
                """));

        if (!response.IsSuccessStatusCode)
        {
            return (false, $"エラーコード: {response.StatusCode}");
        }

        return (true, null);
    }
}

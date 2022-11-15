using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        var request = new HttpRequestMessage(HttpMethod.Post, _appSettingsService.IncomingWebhookUrl)
        {
            Content = new StringContent($$"""
                {
                    "text": "{{message}}"
                }
                """,
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
}

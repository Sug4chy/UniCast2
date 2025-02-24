using Extensions.Hosting.AsyncInitialization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace UniCast.Infrastructure.Telegram.Initialization;

public sealed class SetWebhookAsyncInitializer : IAsyncInitializer
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly string _webhookUrl;

    public SetWebhookAsyncInitializer(ITelegramBotClient telegramBotClient, string webhookUrl)
    {
        _telegramBotClient = telegramBotClient;
        _webhookUrl = webhookUrl;
    }

    public Task InitializeAsync(CancellationToken cancellationToken)
        => _telegramBotClient.SetWebhook(
            url: _webhookUrl,
            allowedUpdates: Update.AllTypes,
            cancellationToken: cancellationToken
        );
}
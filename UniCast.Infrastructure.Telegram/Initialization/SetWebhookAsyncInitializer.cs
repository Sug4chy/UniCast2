using Extensions.Hosting.AsyncInitialization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace UniCast.Infrastructure.Telegram.Initialization;

public sealed class SetWebhookAsyncInitializer : IAsyncInitializer
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly string _webhookUrl;
    private readonly string _certificatePath;

    public SetWebhookAsyncInitializer(
        ITelegramBotClient telegramBotClient,
        string webhookUrl,
        string certificatePath)
    {
        _telegramBotClient = telegramBotClient;
        _webhookUrl = webhookUrl;
        _certificatePath = certificatePath;
    }

    public Task InitializeAsync(CancellationToken cancellationToken)
    {
        var fileStream = File.OpenRead(_certificatePath);

        return _telegramBotClient.SetWebhook(
            url: _webhookUrl,
            allowedUpdates: Update.AllTypes,
            certificate: new InputFileStream(fileStream),
            cancellationToken: cancellationToken
        );
    }
}
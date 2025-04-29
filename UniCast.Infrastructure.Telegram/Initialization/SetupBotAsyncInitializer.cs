using Extensions.Hosting.AsyncInitialization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace UniCast.Infrastructure.Telegram.Initialization;

public sealed class SetupBotAsyncInitializer : IAsyncInitializer
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly string _webhookUrl;
    private readonly string _certificatePath;

    public SetupBotAsyncInitializer(
        ITelegramBotClient telegramBotClient,
        string webhookUrl,
        string certificatePath)
    {
        _telegramBotClient = telegramBotClient;
        _webhookUrl = webhookUrl;
        _certificatePath = certificatePath;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var fileStream = File.OpenRead(_certificatePath);

        await _telegramBotClient.SetWebhook(
            url: _webhookUrl,
            allowedUpdates: Update.AllTypes,
            certificate: new InputFileStream(fileStream),
            cancellationToken: cancellationToken
        );

        await _telegramBotClient.SetMyCommands([
            new BotCommand("/faq", "Получение ответов на часто задаваемые вопросы о факультете"),
            new BotCommand("/order_reference", "Заказать у методиста справку о том. что вы являетесь студентом")
        ], cancellationToken: cancellationToken);
    }
}
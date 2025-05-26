using Extensions.Hosting.AsyncInitialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace UniCast.Infrastructure.Telegram.Initialization;

public sealed class SetupBotAsyncInitializer : IAsyncInitializer
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IWebHostEnvironment _environment;
    private readonly string _webhookUrl;
    private readonly string _certificatePath;

    public SetupBotAsyncInitializer(
        ITelegramBotClient telegramBotClient,
        IWebHostEnvironment environment,
        string webhookUrl,
        string certificatePath)
    {
        _telegramBotClient = telegramBotClient;
        _environment = environment;
        _webhookUrl = webhookUrl;
        _certificatePath = certificatePath;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        // Чтобы при локальном запуске с Tuna не было ошибок от отсутствия сертификата
        if (_environment.IsDevelopment())
        {
            await _telegramBotClient.SetWebhook(
                url: _webhookUrl,
                allowedUpdates: Update.AllTypes,
                cancellationToken: cancellationToken
            );
        }
        else
        {
            var fileStream = File.OpenRead(_certificatePath);

            await _telegramBotClient.SetWebhook(
                url: _webhookUrl,
                allowedUpdates: Update.AllTypes,
                certificate: new InputFileStream(fileStream),
                cancellationToken: cancellationToken
            );
        }

        await _telegramBotClient.SetMyCommands([
            new BotCommand("/faq", "Получение ответов на часто задаваемые вопросы о факультете"),
            new BotCommand("/order_reference", "Заказать у методиста справку о том. что вы являетесь студентом")
        ], cancellationToken: cancellationToken);
    }
}
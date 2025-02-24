using Autofac;
using Telegram.Bot;
using UniCast.Infrastructure.Telegram.Initialization;

namespace UniCast.Infrastructure.Telegram;

public sealed class TelegramInfrastructureModule : Module
{
    public required string BotToken { get; init; }
    public required string WebhookUrl { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadTelegramBotClient(builder);
        LoadTelegramBot(builder);
        LoadSetWebhookAsyncInitializer(builder);
    }

    private void LoadTelegramBotClient(ContainerBuilder builder)
    {
        builder.Register(_ => new TelegramBotClient(BotToken))
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    private static void LoadTelegramBot(ContainerBuilder builder)
    {
        builder.RegisterType<TelegramBot>()
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    private void LoadSetWebhookAsyncInitializer(ContainerBuilder builder)
    {
        builder.Register(ctx => new SetWebhookAsyncInitializer(
            ctx.Resolve<ITelegramBotClient>(),
            WebhookUrl))
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
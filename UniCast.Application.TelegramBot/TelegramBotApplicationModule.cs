using Autofac;
using UniCast.Application.TelegramBot.Handlers;
using UniCast.Application.TelegramBot.Handlers.UserActions;

namespace UniCast.Application.TelegramBot;

public sealed class TelegramBotApplicationModule : Module
{
    public required string BotUsername { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadUpdateDispatcher(builder);
        LoadUpdateHandlers(builder);
    }

    private static void LoadUpdateDispatcher(ContainerBuilder builder)
    {
        builder.RegisterType<UpdateDispatcher>()
            .AsSelf()
            .InstancePerLifetimeScope();
    }

    private void LoadUpdateHandlers(ContainerBuilder builder)
    {
        var handlersTypes = GetType().Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IUpdateHandler)))
            .Except([typeof(BotAddedToChannelUpdateHandler)])
            .ToArray();

        builder.RegisterTypes(handlersTypes)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<BotAddedToChannelUpdateHandler>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .WithParameter(TypedParameter.From(BotUsername));
    }
}
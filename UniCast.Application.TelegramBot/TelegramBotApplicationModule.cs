using Autofac;
using UniCast.Application.TelegramBot.Handlers;

namespace UniCast.Application.TelegramBot;

public sealed class TelegramBotApplicationModule : Module
{
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
            .ToArray();

        builder.RegisterTypes(handlersTypes)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
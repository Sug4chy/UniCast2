using Autofac;
using UniCast.Application.TelegramBot.Handlers;
using UniCast.Application.TelegramBot.Scenarios;

namespace UniCast.Application.TelegramBot;

public sealed class TelegramBotApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        LoadUpdateDispatcher(builder);
        LoadUpdateHandlers(builder);
        LoadScenarioExecutors(builder);
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

    private void LoadScenarioExecutors(ContainerBuilder builder)
    {
        var executorsTypes = GetType().Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IScenarioExecutor)))
            .ToArray();

        builder.RegisterTypes(executorsTypes)
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
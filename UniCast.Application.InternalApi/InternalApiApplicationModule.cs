using Autofac;
using UniCast.Application.InternalApi.Command;

namespace UniCast.Application.InternalApi;

public sealed class InternalApiApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        LoadCommandHandlers(builder);
    }

    private void LoadCommandHandlers(ContainerBuilder builder)
    {
        var commandHandlersTypes = GetType().Assembly
            .GetTypes()
            .Where(x => x.IsClosedTypeOf(typeof(ICommandHandler<>)) ||
                        x.IsClosedTypeOf(typeof(ICommandHandler<,>)))
            .ToArray();

        builder.RegisterTypes(commandHandlersTypes)
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
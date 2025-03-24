using Autofac;
using UniCast.Infrastructure.Persistence.Context;
using UniCast.Infrastructure.Persistence.Context.Options;
using UniCast.Infrastructure.Persistence.Initialization;

namespace UniCast.Infrastructure.Persistence;

public sealed class PersistenceInfrastructureModule : Module
{
    public required string ConnectionString { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadDataContext(builder);
        LoadMigrationsAsyncInitializer(builder);
    }

    private void LoadDataContext(ContainerBuilder builder)
    {
        builder.RegisterType<PostgresqlDataContext>()
            .WithParameter("options", DbContextOptionsFactory.Build<PostgresqlDataContext>(ConnectionString))
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();
    }

    private static void LoadMigrationsAsyncInitializer(ContainerBuilder builder)
    {
        builder.RegisterType<MigrationsAsyncInitializer>()
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
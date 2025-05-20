using Autofac;
using UniCast.Infrastructure.Persistence.Context;
using UniCast.Infrastructure.Persistence.Context.Options;

namespace UniCast.Infrastructure.Persistence;

public sealed class PersistenceInfrastructureModule : Module
{
    public required string ConnectionString { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadDataContext(builder);
    }

    private void LoadDataContext(ContainerBuilder builder)
    {
        builder.RegisterType<PostgresqlDataContext>()
            .WithParameter("options", DbContextOptionsFactory.Build<PostgresqlDataContext>(ConnectionString))
            .AsImplementedInterfaces()
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}
using Autofac;
using UniCast.Infrastructure.Database.Attributes;
using UniCast.Infrastructure.Database.ConnectionFactory;
using UniCast.Infrastructure.Database.Initialization;

namespace UniCast.Infrastructure.Database;

public sealed class DatabaseInfrastructureModule : Module
{
    public required string ConnectionString { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadConnectionFactory(builder);
        LoadRepositories(builder);
        LoadMigrationsAsyncInitializer(builder);
    }

    private void LoadConnectionFactory(ContainerBuilder builder)
    {
        builder.Register(_ => new NpgsqlDbConnectionFactory(ConnectionString))
            .AsImplementedInterfaces()
            .SingleInstance();
    }

    private void LoadRepositories(ContainerBuilder builder)
    {
        var repositoriesTypes = GetType().Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(RepositoryAttribute), false).Length is not 0)
            .ToArray();

        builder.RegisterTypes(repositoriesTypes)
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }

    private void LoadMigrationsAsyncInitializer(ContainerBuilder builder)
    {
        builder.Register(_ => new MigrationsAsyncInitializer(ConnectionString))
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
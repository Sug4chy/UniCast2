using Autofac;
using UniCast.Infrastructure.Moodle.Client;

namespace UniCast.Infrastructure.Moodle;

public sealed class MoodleInfrastructureModule : Module
{
    public required string MoodleBaseUrl { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        LoadMoodleApiClient(builder);
    }

    private void LoadMoodleApiClient(ContainerBuilder builder)
    {
        builder.Register(ctx => new MoodleApiClient(ctx.Resolve<IHttpClientFactory>().CreateClient(), MoodleBaseUrl))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
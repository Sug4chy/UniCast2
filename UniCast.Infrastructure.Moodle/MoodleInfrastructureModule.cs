using Autofac;
using Microsoft.Extensions.Options;
using UniCast.Infrastructure.Moodle.Client;
using UniCast.Infrastructure.Moodle.Configuration;

namespace UniCast.Infrastructure.Moodle;

public sealed class MoodleInfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        LoadMoodleApiClient(builder);
    }

    private static void LoadMoodleApiClient(ContainerBuilder builder)
    {
        builder.Register(ctx => new MoodleApiClient(ctx.Resolve<IHttpClientFactory>().CreateClient(), 
                ctx.Resolve<IOptions<MoodleConfiguration>>()))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
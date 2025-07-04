using Autofac;
using UniCast.Application.Abstractions.Cache;
using UniCast.Infrastructure.Caching.CacheAccessors;

namespace UniCast.Infrastructure.Caching;

public sealed class CachingInfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        LoadCacheAccessors(builder);
    }

    private static void LoadCacheAccessors(ContainerBuilder builder)
    {
        builder.RegisterType<MemoryCacheAccessor>()
            .As<ICacheAccessor>()
            .InstancePerLifetimeScope();
    }
}
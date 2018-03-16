using Autofac;
using Sample.Infrastructure.AggregateCache.InMemory;
using Sample.Infrastructure.EventSourcing.Cache;
using Sample.Infrastructure.Redis;

namespace Sample.Infrastructure.AggregateCache
{
    public static class AggregateCacheModule
    {
        public static ContainerBuilder UseAggregateCache(this ContainerBuilder builder)
        {
            builder.UseRedisAggregateCache();
            builder.RegisterGeneric(typeof(InMemoryAggregateCache<>)).As(typeof(IInMemoryAggregateCache<>)).SingleInstance();
            builder.RegisterGeneric(typeof(AggregateCache<>)).As(typeof(IAggregateCache<>)).SingleInstance();

            return builder;
        }
    }
}

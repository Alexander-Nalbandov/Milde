using Autofac;
using Sample.Infrastructure.AggregateCache.InMemory;
using Sample.Infrastructure.EventSourcing.Hosting;
using Sample.Infrastructure.Redis;

namespace Sample.Infrastructure.AggregateCache
{
    public static class AggregateCacheModule
    {
        public static IEventSourcingConfiguration WithComplexAggregateCache(this IEventSourcingConfiguration configuration)
        {
            return configuration.WithRedisAggregateCache()
                                .WithAggregateCache(typeof(AggregateCache<>), builder =>
                                {
                                    builder
                                        .RegisterGeneric(typeof(InMemoryAggregateCache<>))
                                        .As(typeof(IInMemoryAggregateCache<>))
                                        .SingleInstance();
                                });
        }
    }
}

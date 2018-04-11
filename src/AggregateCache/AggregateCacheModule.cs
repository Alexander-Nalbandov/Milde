using Autofac;
using Milde.AggregateCache.InMemory;
using Milde.EventSourcing.Hosting;
using Milde.Redis;

namespace Milde.AggregateCache
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

using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Cache;

namespace Sample.Infrastructure.Redis.AggregateCache
{
    public interface IRedisAggregateCache<TAggregate> : IAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        
    }
}
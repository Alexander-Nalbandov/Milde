using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Cache;

namespace Milde.Redis.AggregateCache
{
    public interface IRedisAggregateCache<TAggregate> : IAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        
    }
}
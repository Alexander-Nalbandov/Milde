using System.Collections.Generic;
using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Cache;

namespace Sample.Infrastructure.AggregateCache.InMemory
{
    interface IInMemoryAggregateCache<TAggregate> : IAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        void PopulateCache(IList<TAggregate> aggregates);
    }
}
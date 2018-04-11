using System.Collections.Generic;
using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Cache;

namespace Milde.AggregateCache.InMemory
{
    interface IInMemoryAggregateCache<TAggregate> : IAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        void PopulateCache(IList<TAggregate> aggregates);
    }
}
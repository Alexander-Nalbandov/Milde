using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.EventSourcing.Cache
{
    public interface IAggregateCache<TAggregate> 
        where TAggregate : class, IAggregate
    {
        Task Initialize(IEventStore eventStore);

        Task<IList<TAggregate>> GetAllAggregates();
        Task<TAggregate> GetAggregate(Guid id);
        Task SaveAggregate(TAggregate aggregate);
    }
}
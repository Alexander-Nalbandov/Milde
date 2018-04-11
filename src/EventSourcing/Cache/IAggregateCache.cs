using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Events;

namespace Milde.EventSourcing.Cache
{
    public interface IAggregateCache<TAggregate> 
        where TAggregate : class, IAggregate
    {
        Task Initialize(IEventStore eventStore);

        Task<IList<TAggregate>> GetAllAggregates();
        Task<TAggregate> GetAggregate(Guid id);
        Task SaveAggregate(TAggregate aggregate);
        Task SaveAggregates(IList<TAggregate> aggregates);
    }
}
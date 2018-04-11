using System.Collections.Generic;
using System.Threading.Tasks;
using Milde.EventSourcing.Aggregates;

namespace Milde.EventSourcing.Events
{
    public interface IEventStore
    {
        Task<IList<IAggregateEvent>> GetAggregateEvents<TAggregate>() 
            where TAggregate : class, IAggregate;

        Task SaveAggregate<TAggregate>(TAggregate aggregate) 
            where TAggregate : class, IAggregate;

        Task<IList<TAggregate>> UpdateAggregates<TAggregate>(IList<TAggregate> existingAggregates)
            where TAggregate : class, IAggregate;

        Task UpdateAggregate<TAggregate>(TAggregate aggregate)
            where TAggregate : class, IAggregate;
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Infrastructure.EventSourcing.Aggregates;

namespace Sample.Infrastructure.EventSourcing.Events
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
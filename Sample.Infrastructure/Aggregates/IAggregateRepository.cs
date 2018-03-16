using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Infrastructure.EventSourcing.Aggregates
{
    public interface IAggregateRepository<TAggregate> 
        where TAggregate : class, IAggregate
    {
        Task<IList<TAggregate>> Get();
        Task<TAggregate> Get(Guid id);
        Task Save(TAggregate aggregate);
    }
}
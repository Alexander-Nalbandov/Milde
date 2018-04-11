using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Milde.EventSourcing.Cache;
using Milde.EventSourcing.Events;

namespace Milde.EventSourcing.Aggregates
{
    public class AggregateRepository<TAggregate> : IStartable, IAggregateRepository<TAggregate> 
        where TAggregate : class, IAggregate
    {
        public AggregateRepository(IEventStore eventStore, IAggregateCache<TAggregate> cache)
        {
            EventStore = eventStore;
            Cache = cache;
        }

        protected IEventStore EventStore { get; }
        protected IAggregateCache<TAggregate> Cache { get; }


        public void Start()
        {
            this.Cache.Initialize(this.EventStore).Wait();
        }


        public Task<IList<TAggregate>> Get()
        {
            return this.Cache.GetAllAggregates();
        }

        public Task<TAggregate> Get(Guid id)
        {
            return this.Cache.GetAggregate(id);
        }

        public async Task Save(TAggregate aggregate)
        {
            await this.EventStore.SaveAggregate(aggregate);
            await this.Cache.SaveAggregate(aggregate);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.AggregateCache.InMemory
{
    class InMemoryAggregateCache<TAggregate> : IInMemoryAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        private IDictionary<Guid, TAggregate> _aggregates = new Dictionary<Guid, TAggregate>();


        public void PopulateCache(IList<TAggregate> aggregates)
        {
            this._aggregates = aggregates.ToDictionary(aggr => aggr.Id, aggr => aggr);
        }


        public async Task Initialize(IEventStore eventStore)
        {
            // TODO: Optimize

            var events = await eventStore.GetAggregateEvents<TAggregate>();
            var eventsByAggregate = events.GroupBy(e => e.AggregateId);

            foreach (var aggregateEvents in eventsByAggregate)
            {
                var aggregate = Activator.CreateInstance(typeof(TAggregate), aggregateEvents.Key) as TAggregate;
                var eventsToApply = aggregateEvents.OrderBy(e => e.Version);

                foreach (var @event in eventsToApply)
                {
                    aggregate.ApplyEvent(@event);
                }

                await this.SaveAggregate(aggregate);
            }
        }


        public Task<IList<TAggregate>> GetAllAggregates()
        {
            return Task.FromResult<IList<TAggregate>>(this._aggregates.Values.ToList());
        }

        public Task<TAggregate> GetAggregate(Guid id)
        {
            if (this._aggregates.TryGetValue(id, out TAggregate aggregate))
            {
                return Task.FromResult(aggregate);
            }

            return Task.FromResult<TAggregate>(null);
        }

        public Task SaveAggregate(TAggregate aggregate)
        {
            this._aggregates[aggregate.Id] = aggregate;
            return Task.CompletedTask;
        }
    }
}

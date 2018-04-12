using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Events;

namespace Milde.AggregateCache.InMemory
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

        public async Task<IList<TAggregate>> GetAggregates(Func<TAggregate, bool> predicate)
        {
            return this._aggregates.Values.Where(predicate).ToList();
        }

        public Task SaveAggregate(TAggregate aggregate)
        {
            return this.SaveAggregates(new[] {aggregate});
        }

        public Task SaveAggregates(IList<TAggregate> aggregates)
        {
            foreach (var aggregate in aggregates)
            {
                this._aggregates[aggregate.Id] = aggregate;
            }

            return Task.CompletedTask;
        }
    }
}

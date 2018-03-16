using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.EventBus;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.EventStore.EventStore
{
    internal class EventStore : IEventStore
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventConverter _converter;
        private readonly IEventBus _eventBus;


        public EventStore(IEventStoreConnection connection, EventConverter converter, IEventBus eventBus)
        {
            _connection = connection;
            _converter = converter;
            _eventBus = eventBus;
        }


        public async Task<IList<IAggregateEvent>> GetAggregateEvents(string stream)
        {
            var eventDatas = await this._connection.ReadStreamEventsForwardAsync(stream, 0, ClientApiConstants.MaxReadSize, false);
            return eventDatas.Events.Select(e => this._converter.FromEventData(e.Event)).ToList();
        }

        public Task<IList<IAggregateEvent>> GetAggregateEvents<TAggregate>() where TAggregate : class, IAggregate
        {
            var stream = this.GetStream<TAggregate>();
            return this.GetAggregateEvents(stream);
        }

        public async Task SaveAggregate<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregate
        {
            var changes = aggregate.GetUncommitedChanges();
            var events = changes.Select(c => this._converter.ToEventData(c));

            var stream = this.GetStream<TAggregate>();
            using (var transaction = await this._connection.StartTransactionAsync(stream, ExpectedVersion.Any))
            {
                await transaction.WriteAsync(events);
                await transaction.CommitAsync();
            }

            await this._eventBus.Publish(changes);

            aggregate.MarkAsCommited();
        }


        private string GetStream<TAggregate>() where TAggregate : class, IAggregate
        {
            return typeof(TAggregate).Name;
        }
    }
}

using System;
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
        

        public async Task<IList<IAggregateEvent>> GetAggregateEvents<TAggregate>()
            where TAggregate : class, IAggregate
        {
            var stream = typeof(TAggregate).Name;
            var events = await this.GetEvents(stream);

            return events.Select(e => this._converter.FromEventData(e.Event)).ToList();
        }


        public async Task<IList<TAggregate>> UpdateAggregates<TAggregate>(IList<TAggregate> existingAggregates)
            where TAggregate : class, IAggregate
        {
            var aggregates = new List<TAggregate>();
            var aggregateIds = (await this.GetEvents(this.GetAggregateIDsStream<TAggregate>())).Select(e => e.Event.EventId);

            Parallel.ForEach(aggregateIds, async aggregateId =>
            {
                var aggregate = existingAggregates.SingleOrDefault(a => a.Id == aggregateId) ??
                                (TAggregate) Activator.CreateInstance(typeof(TAggregate), aggregateId);

                await this.UpdateAggregate(aggregate);

                aggregates.Add(aggregate);
            });

            return aggregates;
        }

        public async Task UpdateAggregate<TAggregate>(TAggregate aggregate)
            where TAggregate : class, IAggregate
        {
            var aggregateEvents = await this.GetEvents(
                stream: this.GetAggregateStream(aggregate),
                fromVersion: aggregate.Version
            );

            aggregateEvents.Select(e => this._converter.FromEventData(e.Event))
                           .OrderBy(e => e.Version)
                           .ToList()
                           .ForEach(e => aggregate.ApplyEvent(e));
        }


        public async Task SaveAggregate<TAggregate>(TAggregate aggregate) 
            where TAggregate : class, IAggregate
        {
            var changes = aggregate.GetUncommitedChanges();
            var events = changes.Select(c => this._converter.ToEventData(c)).ToList();

            await this.SaveEvents(
                stream: typeof(TAggregate).Name,
                events: events
            );

            await this.SaveEvents(
                stream: this.GetAggregateStream(aggregate),
                events: events,
                expectedVersion: aggregate.Version - events.Count - 1
            );

            await this.SaveEvents(
                stream: this.GetAggregateIDsStream<TAggregate>(),
                events: new[] {new EventData(aggregate.Id, typeof(Guid).Name, false, null, null)}
            );

            await this._eventBus.Publish(changes);

            aggregate.MarkAsCommited();
        }



        private string GetAggregateIDsStream<TAggregate>() where TAggregate : class, IAggregate 
            => $"{typeof(TAggregate).Name}-Ids";

        private string GetAggregateStream<TAggregate>(TAggregate aggregate) where TAggregate : class, IAggregate
            => $"{typeof(TAggregate).Name}-{aggregate.Id}";



        private async Task<IList<ResolvedEvent>> GetEvents(string stream, long fromVersion = 0)
        {
            var events = new List<ResolvedEvent>();

            StreamEventsSlice eventDatas = null;
            do
            {
                eventDatas = await this._connection.ReadStreamEventsForwardAsync(
                    stream: stream,
                    start: eventDatas?.NextEventNumber ?? fromVersion,
                    count: ClientApiConstants.MaxReadSize,
                    resolveLinkTos: false
                );
                events.AddRange(eventDatas.Events);
            } while (!eventDatas.IsEndOfStream);

            return events;
        }

        private async Task SaveEvents(string stream, IList<EventData> events, long expectedVersion = ExpectedVersion.Any)
        {
            using (var transaction = await this._connection.StartTransactionAsync(stream, expectedVersion))
            {
                await transaction.WriteAsync(events);
                await transaction.CommitAsync();
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Sample.Infrastructure.EventSourcing.EventBus;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.EventStore.EventBus
{
    class EventBus : IEventBus
    {
        private readonly IEventStoreConnection _connection;
        private readonly EventConverter _converter;


        public EventBus(IEventStoreConnection connection, EventConverter converter)
        {
            _connection = connection;
            _converter = converter;
        }


        public Task Publish<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent
        {
            var stream = this.GetStream(@event.GetType());
            var eventData = this._converter.ToEventData(@event);
            return this._connection.AppendToStreamAsync(stream, ExpectedVersion.Any, eventData);
        }

        public async Task Publish<TEvent>(IList<TEvent> events) where TEvent : class, IAggregateEvent
        {
            var typedEvents = (
                from @event in events
                group @event by @event.GetType()
                into eventsByType
                select new
                {
                    EventType = eventsByType.Key,
                    Events = eventsByType.ToList()
                }
            );

            foreach (var eventsByType in typedEvents)
            {
                var stream = this.GetStream(eventsByType.EventType);
                var eventDatas = eventsByType.Events.Select(e => this._converter.ToEventData(e));
                await this._connection.AppendToStreamAsync(stream, ExpectedVersion.Any, eventDatas);
            }
        }

        public Task Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IAggregateEvent
        {
            var stream = this.GetStream(typeof(TEvent));
            return this._connection.SubscribeToStreamAsync(stream, false, (s, eventData) =>
            {
                var @event = this._converter.FromEventData<TEvent>(eventData.Event);
                handler(@event).Wait();
            });
        }

        public Task Subscribe(Type eventType, Func<IAggregateEvent, Task> handler)
        {
            var stream = this.GetStream(eventType);
            return this._connection.SubscribeToStreamAsync(stream, false, (s, eventData) =>
            {
                var @event = this._converter.FromEventData(eventType, eventData.Event);
                handler(@event).Wait();
            });
        }

        
        private string GetStream(Type eventType)
        {
            return eventType.Name;
        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using EventStore.ClientAPI;
using Sample.Infrastructure.EventSourcing.Events;
using Sample.Infrastructure.EventSourcing.Serialization;
using Sample.Infrastructure.EventStore.Entities;

namespace Sample.Infrastructure.EventStore
{
    internal class EventConverter
    {
        private readonly ISerializer _serializer;


        public EventConverter(ISerializer serializer)
        {
            _serializer = serializer;
        }


        public EventData ToEventData(IAggregateEvent @event)
        {
            var dataJson = this._serializer.Serialize(@event);
            var data = this._serializer.SerializeToBytes(dataJson);

            var metadataJson = this._serializer.Serialize(new EventMetadata(
                assembly: @event.GetType().Assembly.Location,
                eventType:@event.GetType().Name)
            );
            var metadata = this._serializer.SerializeToBytes(metadataJson);

            return new EventData(
                eventId: Guid.NewGuid(),
                type: @event.GetType().Name,
                isJson: true,
                data: data,
                metadata: metadata
            );
        }

        public TEvent FromEventData<TEvent>(RecordedEvent eventData)
            where TEvent : class, IAggregateEvent
        {
            var json = this._serializer.DeserializeFromBytes(eventData.Data);
            return this._serializer.Deserialize<TEvent>(json);
        }

        public IAggregateEvent FromEventData(Type eventType, RecordedEvent eventData)
        {
            var json = this._serializer.DeserializeFromBytes(eventData.Data);
            return this._serializer.Deserialize(eventType, json) as IAggregateEvent;
        }

        public IAggregateEvent FromEventData(RecordedEvent eventData)
        {
            var metadataJson = this._serializer.DeserializeFromBytes(eventData.Metadata);
            var metadata = this._serializer.Deserialize<EventMetadata>(metadataJson);

            // TODO: Change from using full path to assembly and use relative path
            var eventType = Assembly.LoadFrom(metadata.Assembly).DefinedTypes.Single(t => t.Name == metadata.EventType);
            return this.FromEventData(eventType, eventData);
        }
    }
}

using System;
using Sample.Infrastructure.EventSourcing.Messaging;

namespace Sample.Infrastructure.EventSourcing.Events
{
    public interface IAggregateEvent : IMessage
    {
        Guid AggregateId { get; }
        int Version { get; }
        int ShardKey { get; }
    }
}

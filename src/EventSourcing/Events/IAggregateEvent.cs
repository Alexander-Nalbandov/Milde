using System;
using Milde.EventSourcing.Messaging;

namespace Milde.EventSourcing.Events
{
    public interface IAggregateEvent : IMessage
    {
        Guid AggregateId { get; }
        int Version { get; }
        int ShardKey { get; }
    }
}

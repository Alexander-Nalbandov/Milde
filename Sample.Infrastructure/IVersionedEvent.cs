using System;

namespace Sample.Infrastructure.EventSourcing
{
    public interface IVersionedEvent : IEvent
    {
        Guid Id { get; }
        int ShardKey { get; }
        int Version { set; }
    }
}
using System;

namespace Sample.Infrastructure.EventSourcing
{
    public interface IVersionedEvent : IEvent
    {
        Guid Id { get; }
        int Version { set; }
    }
}
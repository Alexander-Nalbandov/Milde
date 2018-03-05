using System;

namespace Sample.Infrastructure.Messaging
{
    public interface IEvent
    {
        Guid Id { get; }
        int Version { set; }
    }
}
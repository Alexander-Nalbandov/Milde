using System;
using System.Collections.Generic;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.EventSourcing.Aggregates
{
    public interface IAggregate
    {
        Guid Id { get; }
        int ShardKey { get; }
        int Version { get; }

        void ApplyEvent(IAggregateEvent @event);

        IList<IAggregateEvent> GetUncommitedChanges();
        void MarkAsCommited();
    }
}
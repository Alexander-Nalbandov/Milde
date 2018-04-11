using System;

namespace Milde.EventSourcing.Events
{
    public abstract class AggregateEvent : IAggregateEvent
    {
        protected AggregateEvent(Guid aggregateId, int version, int shardKey)
        {
            AggregateId = aggregateId;
            Version = version;
            ShardKey = shardKey;
        }

        public Guid AggregateId { get; private set; }
        public int Version { get; private set; }
        public int ShardKey { get; private set; }
    }
}
using System;
using System.Collections.Generic;
using Milde.EventSourcing.Events;

namespace Milde.EventSourcing.Aggregates
{
    public abstract class Aggregate : IAggregate
    {
        public const int MaxShardKey = 10000;
        private static readonly Random _shardKeyGenerator = new Random();
        private readonly IList<IAggregateEvent> _changes = new List<IAggregateEvent>();
        

        protected Aggregate(Guid id)
            : this(id, GenerateShardKey())
        {
        }

        protected Aggregate(Guid id, int shardKey)
        {
            Id = id;
            Version = 0;
            ShardKey = shardKey;
        }


        // TODO: Remove it and make proper deserialization
        protected Aggregate(Guid id, int version, int shardKey)
        {
            Id = id;
            Version = version;
            ShardKey = shardKey;
        }


        public Guid Id { get; private set; }

        public int Version { get; private set; }

        public int ShardKey { get; private set; }


        public IList<IAggregateEvent> GetUncommitedChanges()
        {
            return this._changes;
        }

        public void MarkAsCommited()
        {
            this._changes.Clear();
        }


        public void ApplyEvent(IAggregateEvent @event)
        {
            if (this.Id != @event.AggregateId)
            {
                return;
            }

            if (this.Version + 1 != @event.Version)
            {
                throw new InvalidOperationException();
            }

            this.Version = @event.Version;

            if (@event.Version == 1)
            {
                this.ShardKey = @event.ShardKey;
            }
            else if (this.ShardKey != @event.ShardKey)
            {
                throw new InvalidOperationException();
            }

            this.CallApply(@event);

            this._changes.Add(@event);
        }

        protected abstract void CallApply(IAggregateEvent @event);

        private static int GenerateShardKey()
        {
            return _shardKeyGenerator.Next(0, MaxShardKey);
        }
    }
}

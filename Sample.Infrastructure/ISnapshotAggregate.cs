using System;

namespace Sample.Infrastructure.EventSourcing
{
    public interface ISnapshotAggregate<TState> : IAggregate
    {
        TState GetState();

        void Initialize(Guid aggregatedId, int version, TState state);
    }
}
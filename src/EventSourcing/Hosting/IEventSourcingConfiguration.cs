using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Milde.EventSourcing.Events;

namespace Milde.EventSourcing.Hosting
{
    public interface IEventSourcingConfiguration
    {
        IEventSourcingConfiguration WithEventStore<TEventStore>()
            where TEventStore : IEventStore;

        IEventSourcingConfiguration WithEventStore<TEventStore>(Action<ContainerBuilder> registerAdditionalDependencies)
            where TEventStore : IEventStore;

        IEventSourcingConfiguration WithEventStore<TEventStore>(
            Action<ContainerBuilder, IConfiguration> registerAdditionalDependencies
        ) where TEventStore : IEventStore;

        IEventSourcingConfiguration WithAggregateCache(Type aggregateCacheType);

        IEventSourcingConfiguration WithAggregateCache(
            Type aggregateCacheType,
            Action<ContainerBuilder> registerAdditionalDependencies
        );

        IEventSourcingConfiguration WithAggregateCache(
            Type aggregateCacheType,
            Action<ContainerBuilder, IConfiguration> registerAdditionalDependencies
        );
    }
}
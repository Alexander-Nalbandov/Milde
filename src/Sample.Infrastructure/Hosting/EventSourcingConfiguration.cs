using System;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.EventSourcing.Cache;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.EventSourcing.Hosting
{
    internal class EventSourcingConfiguration : IEventSourcingConfiguration
    {
        private readonly ContainerBuilder _builder;
        private readonly IConfiguration _configuration;


        public EventSourcingConfiguration(ContainerBuilder builder, IConfiguration configuration)
        {
            _builder = builder;
            _configuration = configuration;
        }


        public IEventSourcingConfiguration WithEventStore<TEventStore>() where TEventStore : IEventStore
        {
            return this.WithEventStore<TEventStore>((builder, config) => { });
        }

        public IEventSourcingConfiguration WithEventStore<TEventStore>(Action<ContainerBuilder> registerAdditionalDependencies) 
            where TEventStore : IEventStore
        {
            return this.WithEventStore<TEventStore>((builder, config) => registerAdditionalDependencies(builder));
        }

        public IEventSourcingConfiguration WithEventStore<TEventStore>(Action<ContainerBuilder, IConfiguration> registerAdditionalDependencies) where TEventStore : IEventStore
        {
            this._builder.RegisterType<TEventStore>().AsImplementedInterfaces().SingleInstance();
            registerAdditionalDependencies?.Invoke(this._builder, this._configuration);

            return this;
        }

        public IEventSourcingConfiguration WithAggregateCache(Type aggregateCacheType)
        {
            return this.WithAggregateCache(aggregateCacheType, (builder, config) => { });
        }

        public IEventSourcingConfiguration WithAggregateCache(
            Type aggregateCacheType,
            Action<ContainerBuilder> registerAdditionalDependencies
        )
        {
            return this.WithAggregateCache(
                aggregateCacheType,
                (builder, config) => registerAdditionalDependencies(builder)
            );
        }

        public IEventSourcingConfiguration WithAggregateCache(Type aggregateCacheType, Action<ContainerBuilder, IConfiguration> registerAdditionalDependencies)
        {
            bool ImplementsIAggregateCacheInterface(Type type)
            {
                return type.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IAggregateCache<>)
                );
            }

            if (!aggregateCacheType.IsGenericType ||
                aggregateCacheType.IsConstructedGenericType ||
                !ImplementsIAggregateCacheInterface(aggregateCacheType)
            )
            {
                throw new Exception("Invalid type was provided as Aggregate Cache implementation");
            }

            this._builder.RegisterGeneric(aggregateCacheType).As(typeof(IAggregateCache<>)).SingleInstance();
            registerAdditionalDependencies?.Invoke(this._builder, this._configuration);

            return this;
        }
    }
}
using System.Net;
using System.Reflection;
using Autofac;
using EventStore.ClientAPI;
using Sample.Infrastructure.EventSourcing.EventBus;
using Sample.Infrastructure.EventSourcing.Hosting;
using Sample.Infrastructure.EventSourcing.Serialization;
using Sample.Infrastructure.EventStore.Configuration;
using Sample.Infrastructure.Hosting.Host;

namespace Sample.Infrastructure.EventStore
{
    public static class EventStoreModule
    {
        public static IHostBuilder WithEventStoreEventBus(this IHostBuilder hostBuilder, params Assembly[] handlersAssemblies)
        {
            return hostBuilder.With((builder, config) =>
            {
                var eventStoreConfig = new EventStoreConfiguration(config);

                builder.WithEventStoreDatabase(eventStoreConfig);
                builder.RegisterType<EventBus.EventBus>().AsImplementedInterfaces().SingleInstance();
                builder.RegisterType<EventConverter>().AsSelf().SingleInstance();

                builder.RegisterType<Serializer>().AsImplementedInterfaces().SingleInstance();

                builder.RegisterEventHandlers(handlersAssemblies);
            });
        }

        public static IEventSourcingConfiguration WithEventStoreAsES(
            this IEventSourcingConfiguration configuration
        )
        {
            return configuration.WithEventStore<EventStore.EventStore>((builder, config) =>
            {
                var eventStoreConfig = new EventStoreConfiguration(config);
                
                builder.WithEventStoreDatabase(eventStoreConfig);
                builder.RegisterType<EventConverter>().AsSelf().SingleInstance();
            });
        }


        private static void WithEventStoreDatabase(this ContainerBuilder builder, EventStoreConfiguration config)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(
                address: IPAddress.Parse(config.Host),
                port: config.Port
            ));
            connection.ConnectAsync().Wait();
            builder.RegisterInstance(connection).AsImplementedInterfaces().SingleInstance();
        }
    }
}

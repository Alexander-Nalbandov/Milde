using System.Net;
using Autofac;
using EventStore.ClientAPI;
using Sample.Infrastructure.EventStore.Configuration;

namespace Sample.Infrastructure.EventStore
{
    public static class EventStoreModule
    {
        public static ContainerBuilder UseEventStore(this ContainerBuilder builder, EventStoreConfiguration configuration)
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(
                address: IPAddress.Parse(configuration.Host),
                port: configuration.Port
            ));
            connection.ConnectAsync().Wait();
            builder.RegisterInstance(connection).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<EventStore.EventStore>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<EventConverter>().AsSelf().SingleInstance();

            builder.RegisterType<EventBus.EventBus>().AsImplementedInterfaces().SingleInstance();

            return builder;
        }
    }
}

using Autofac;
using Sample.Infrastructure.EventSourcing.Serialization;

namespace Sample.Infrastructure.EventSourcing
{
    public static class InfrastructureModule
    {
        public static ContainerBuilder UseSerializer(this ContainerBuilder builder)
        {
            builder.RegisterType<Serializer>().AsImplementedInterfaces().SingleInstance();
            return builder;
        }
    }
}

using Autofac;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Configuration;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Rabbit.Registration
{
    internal class RabbitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ResponseAwaitersRegistry<RemoteResponse>>()
                .AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializer>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<RabbitConfiguration>()
                .AsSelf().SingleInstance();
            builder.RegisterType<RabbitConnectionFactory>()
                .AsSelf().SingleInstance();
            base.Load(builder);
        }
    }
}
using Autofac;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Serialization;

namespace Sample.Infrastructure.Remoting.Registration
{
    public class RemotingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ResponseAwaitersRegistry<RemoteResponse>>()
                .AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializer>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ResponseConverter>()
                .AsSelf().SingleInstance();
            base.Load(builder);
        }
    }
}
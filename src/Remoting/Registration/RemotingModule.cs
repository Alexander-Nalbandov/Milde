using Autofac;
using Milde.Remoting.Client;
using Milde.Remoting.Communication;
using Milde.Remoting.Serialization;

namespace Milde.Remoting.Registration
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
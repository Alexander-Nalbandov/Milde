using Autofac;
using Milde.Remoting.Rabbit.Communication;
using Milde.Remoting.Rabbit.Configuration;
using Milde.Remoting.Registration;

namespace Milde.Remoting.Rabbit.Registration
{
    internal class RabbitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<RemotingModule>();
            builder.RegisterType<RabbitConfiguration>()
                .AsSelf().SingleInstance();
            builder.RegisterType<RabbitConnectionFactory>()
                .AsSelf().SingleInstance();
            base.Load(builder);
        }
    }
}
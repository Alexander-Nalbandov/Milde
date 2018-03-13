using Autofac;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Rabbit.Communication;

namespace Sample.Infrastructure.Remoting.Rabbit.Registration
{
    public class ServiceConfigurator
    {
        private readonly ContainerBuilder _builder;

        internal ServiceConfigurator(ContainerBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        public void RegisterProxy<TInterface>()
        {
            _builder.RegisterType<RabbitListener<TInterface, RemoteResponse>>()
                .WithParameter("exchange", "RPC-Response")
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            _builder.RegisterType<RabbitSender<TInterface, RemoteRequest>>()
                .WithParameter("exchange", "RPC-Request")
                .AsSelf().AsImplementedInterfaces().SingleInstance();
            _builder.RegisterType<RemoteProcedureExecutor<TInterface>>()
                .AsSelf().SingleInstance();
            _builder.Register(cc =>
                    ServiceProxyFactory.Create<TInterface>(cc.Resolve<RemoteProcedureExecutor<TInterface>>()))
                .AsImplementedInterfaces().SingleInstance();
        }
    }
}
using System;
using System.Collections.Generic;
using Autofac;
using Sample.Infrastructure.Remoting.Client;

namespace Sample.Infrastructure.Remoting.RabbitMq
{
    public static class ContainerExtensions
    {
        public static ContainerBuilder WithRabbitRemoting(
            this ContainerBuilder builder, Action<ServiceConfigurator> action)
        {
            var configurator = new ServiceConfigurator();
            action(configurator);
            configurator.AttachRegistrations(builder);

            return builder;
        }


        public class ServiceConfigurator
        {
            private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

            public void Register<TInterface>()
            {
                _services.Add(typeof(TInterface),  ServiceProxyFactory.Create<TInterface>());
            }

            internal void AttachRegistrations(ContainerBuilder builder)
            {
                foreach (var service in _services)
                {
                    builder.RegisterInstance(service.Value).AsImplementedInterfaces().SingleInstance();
                }
            }
        }
    }


}
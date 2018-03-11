using System;
using Autofac;

namespace Sample.Infrastructure.Remoting.RabbitMq
{
    public static class ContainerExtensions
    {
        public static ContainerBuilder WithRabbitRemoting(
            this ContainerBuilder builder, Action<bool> action)
        {
            builder.RegisterInstance()
            return builder;
        }
    }
}
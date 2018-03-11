using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.Remoting.RabbitMq;

namespace Sample.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                {"rabbit_url", "amqp://localhost:1526"}
            });

            var builder = new ContainerBuilder();
            builder.RegisterInstance(configBuilder.Build())
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.WithRabbitRemoting();

            builder.Build();
        }
    }
}

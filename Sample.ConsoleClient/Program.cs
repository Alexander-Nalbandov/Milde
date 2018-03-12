using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.Remoting.RabbitMq;
using Sample.UserManagement.Contract;

namespace Sample.ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"rabbit_url", "amqp://localhost:1526"}
            });

            var builder = new ContainerBuilder();
            builder.RegisterInstance(configBuilder.Build())
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.WithRabbitRemoting(configurator =>
                {
                    configurator.Register<IUserManagementService>();
                }
            );

            builder.Build();
        }
    }
}
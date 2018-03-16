using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.AggregateCache;
using Sample.Infrastructure.EventSourcing;
using Sample.Infrastructure.EventStore;
using Sample.Infrastructure.EventStore.Configuration;
using Sample.Infrastructure.Redis;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Handlers;
using Serilog;

namespace Sample.ConsoleClient
{
    internal class SampleClientProgram
    {
        public void Run()
        {
            var cfg = BuildConfiguration();
            var container = ConfigureContainer(cfg);
            var service = container.Resolve<IUserManagementService>();
            while (true)
            {
                Console.WriteLine("First Name:");
                var firstName = Console.ReadLine();

                Console.WriteLine("Last Name:");
                var lastName = Console.ReadLine();

                Console.WriteLine("Age:");
                var age = int.Parse(Console.ReadLine());

                var a = service.CreateUser(firstName, lastName, age).Result;
                Console.WriteLine($"Created {a}");
            }
        }

        public IContainer ConfigureContainer(IConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(config)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.WithRabbitRemoting(configurator =>
                {
                    configurator.RegisterProxy<IUserManagementService>();
                }
            );

            return builder.Build();
        }

        public IConfigurationRoot BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"rabbit:address", "amqp://localhost:5672"},
                {"rabbit:username", "client"},
                {"rabbit:password", "password"},
                {"rabbit:vhost", "/"}
            });
            return configBuilder.Build();
        }
    }
}
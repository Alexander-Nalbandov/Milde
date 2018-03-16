using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.AggregateCache;
using Sample.Infrastructure.EventSourcing;
using Sample.Infrastructure.EventStore;
using Sample.Infrastructure.EventStore.Configuration;
using Sample.Infrastructure.Redis;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
using Sample.UserManagement;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Handlers;
using Serilog;

namespace Sample.ConsoleServer
{
    class SampleServerProgram
    {
        public void Run()
        {
            var cfg = BuildConfiguration();
            var container = ConfigureContainer(cfg);

            Console.ReadKey();
        }

        public IContainer ConfigureContainer(IConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(config)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.WithRabbitRemoting(configurator =>
                {
                    configurator.RegisterService<UserManagementService, IUserManagementService>();
                }
            );

            builder.UseSerializer();

            var redisConnection = config["Redis:ConnectionString"];
            builder.UseRedis(redisConnection);

            builder.UseAggregateCache();

            builder.UseEventStore(new EventStoreConfiguration
            {
                Host = config["EventStore:Host"],
                Port = int.Parse(config["EventStore:Port"])
            });

            builder.RegisterModule<UserServiceModule>();

            var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            builder.RegisterInstance(logger).AsImplementedInterfaces().SingleInstance();

            return builder.Build();
        }

        public IConfigurationRoot BuildConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"rabbit:address", "amqp://localhost:5672"},
                {"rabbit:username", "server"},
                {"rabbit:password", "password"},
                {"rabbit:vhost", "/"},
                {"Redis:ConnectionString", "localhost:6379"},
                {"EventStore:Host", "127.0.0.1"},
                {"EventStore:Port", "1113"}
            });
            return configBuilder.Build();
        }
    }
}

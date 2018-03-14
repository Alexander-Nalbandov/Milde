using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
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
                {"rabbit:username", "username"},
                {"rabbit:password", "password"},
                {"rabbit:vhost", "/"},
            });
            return configBuilder.Build();
        }
    }
}

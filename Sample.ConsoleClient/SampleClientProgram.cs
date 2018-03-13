using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
using Sample.UserManagement.Contract;

namespace Sample.ConsoleClient
{
    internal class SampleClientProgram
    {
        public void Run()
        {
            var cfg = BuildConfiguration();
            var container = ConfigureContainer(cfg);
            var service = container.Resolve<IUserManagementService>();
            service.CreateUser("asd");
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
                {"rabbit:username", "username"},
                {"rabbit:password", "password"},
                {"rabbit:vhost", "/"},
            });
            return configBuilder.Build();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
using Sample.UserManagement.Contract;

namespace Sample.ConsoleClient
{
    internal class SampleClientProgram
    {
        public async void Run()
        {
            var cfg = BuildConfiguration();
            var container = ConfigureContainer(cfg);
            var service = container.Resolve<IUserManagementService>();
            while (true)
            {
                var command = Console.ReadLine();
                switch (command)
                {
                    case "list":
                        await this.ListCommand(service);
                        break;
                    case "create":
                        await this.CreateCommand(service);
                        break;
                    case "throw":
                        await service.ThrowException(Guid.Empty);
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
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
                {"rabbit:username", "guest"},
                {"rabbit:password", "guest"},
                {"rabbit:vhost", "/"}
            });
            return configBuilder.Build();
        }


        private async Task ListCommand(IUserManagementService service)
        {
            var users = await service.Get();
            if (!users.Any())
            {
                Console.WriteLine("There are no users.");
                return;
            }

            foreach (var user in users)
            {
                Console.WriteLine(JsonConvert.SerializeObject(user));
            }
        }

        private async Task CreateCommand(IUserManagementService service)
        {
            Console.WriteLine("First Name:");
            var firstName = Console.ReadLine();

            Console.WriteLine("Last Name:");
            var lastName = Console.ReadLine();

            Console.WriteLine("Age:");
            var age = int.Parse(Console.ReadLine());

            var a = await service.CreateUser(firstName, lastName, age);
            Console.WriteLine($"Created {JsonConvert.SerializeObject(a)}");
        }
    }
}
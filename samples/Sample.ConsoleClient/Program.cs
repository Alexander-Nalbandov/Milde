using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.EventStore;
using Sample.Infrastructure.Hosting.Host;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Aggregates;
using Serilog;

namespace Sample.ConsoleClient
{
    internal class Program
    {
        private static void Main()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());

            var host = HostBuilder
                       .Create()
                       .WithConfiguration(configBuilder =>
                       {
                           configBuilder.AddInMemoryCollection(new Dictionary<string, string>
                           {
                               {"rabbit:address", "amqp://localhost:5672"},
                               {"rabbit:username", "guest"},
                               {"rabbit:password", "guest"},
                               {"rabbit:vhost", "/"},
                               {"EventStore:Host", "127.0.0.1"},
                               {"EventStore:Port", "1113"}
                           });
                       })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       .WithEventStoreEventBus(Assembly.GetExecutingAssembly())
                       .WithRabbitRemoting(configurator =>
                       {
                           configurator.RegisterProxy<IUserManagementService>();
                       })
                       .Build();

            host.Run((container, configuration) =>
            {
                Console.WriteLine("Client has started.");

                new SampleClientProgram().Run(container);

                return Task.CompletedTask;
            }).Wait();

        }
    }
}
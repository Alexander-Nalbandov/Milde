using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.EventStore;
using Sample.Infrastructure.Hosting.Host;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Aggregates;
using Sample.UserManagement.Handlers;
using Serilog;

namespace Sample.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());

            var host = HostBuilder
                       .Create()
                       .WithConfiguration(configBuilder =>
                       {
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
                       })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       .WithEventStoreEventBus()
                       .WithService<UserManagementService>()
                       .Build();

            host.Run((container, configuration) =>
            {
                Console.WriteLine("Server has started.");
                Console.ReadKey();

                return Task.CompletedTask;
            }).Wait();
        }
    }
}

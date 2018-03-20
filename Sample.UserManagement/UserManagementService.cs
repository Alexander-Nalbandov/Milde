using Autofac;
using Microsoft.Extensions.Configuration;
using Sample.Infrastructure.AggregateCache;
using Sample.Infrastructure.EventSourcing.Hosting;
using Sample.Infrastructure.EventStore;
using Sample.Infrastructure.Hosting;
using Sample.Infrastructure.Hosting.Service;
using Sample.Infrastructure.Remoting.Rabbit.Registration;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Repositories;
using Serilog;

// ReSharper disable once CheckNamespace
namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService : IService, IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;


        public UserManagementService()
        {
        }


        public UserManagementService(ILogger logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }


        public void RegisterDependencies(ContainerBuilder builder, IConfiguration config)
        {
            builder.WithEventSourcing(eventSourcingConfig =>
                   {
                       eventSourcingConfig.WithComplexAggregateCache()
                                          .WithEventStoreAsES();
                   }, config)
                   .WithRabbitRemoting(configurator =>
                   {
                       configurator.RegisterService<UserManagementService, IUserManagementService>();
                   });
            
            builder.RegisterType<UserRepository>().AsImplementedInterfaces().SingleInstance();
        }
    }
}

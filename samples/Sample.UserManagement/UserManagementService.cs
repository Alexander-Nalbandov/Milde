using Autofac;
using Microsoft.Extensions.Configuration;
using Milde.AggregateCache;
using Milde.EventSourcing.Hosting;
using Milde.EventStore;
using Milde.Hosting.Service;
using Milde.Remoting.Rabbit.Registration;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Repositories;
using Serilog;

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
                       AggregateCacheModule.WithComplexAggregateCache(eventSourcingConfig)
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

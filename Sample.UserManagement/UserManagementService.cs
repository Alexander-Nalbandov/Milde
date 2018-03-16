using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Repositories;
using Serilog;

// ReSharper disable once CheckNamespace
namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public UserManagementService(ILogger logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
    }
}

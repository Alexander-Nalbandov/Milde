using System.Threading.Tasks;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain;
using Serilog;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService : IUserManagementService
    {
        private readonly ILogger _logger;

        public UserManagementService(ILogger logger)
        {
            _logger = logger;
        }

        public Task<UserDto> CreateUser(string name)
        {
            var user = User.Create(name);
            _logger.Information("Created user: {@User}", user);
            return Task.FromResult(user.ToDto());
        }
    }
}
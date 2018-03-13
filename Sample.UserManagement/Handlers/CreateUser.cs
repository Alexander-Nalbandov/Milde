using System.Threading.Tasks;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService : IUserManagementService
    {
        public Task<UserDto> CreateUser(string name)
        {
            var user = User.Create(name);
            return Task.FromResult(user.ToDto());
        }
    }
}
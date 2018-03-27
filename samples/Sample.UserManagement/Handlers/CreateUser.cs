using System.Threading.Tasks;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Aggregates;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService : IUserManagementService
    {
        public async Task<UserDto> CreateUser(string firstName, string lastName, long age)
        {
            var user = User.Create(firstName, lastName, (int)age);

            await this._userRepository.Save(user);

            this._logger.Information("Created user: {@User}", user);

            return user.ToDto();
        }
    }
}
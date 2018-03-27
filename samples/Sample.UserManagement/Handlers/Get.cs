using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sample.UserManagement.Contract;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService : IUserManagementService
    {
        public async Task<IList<UserDto>> Get()
        {
            this._logger.Information("Getting list of all users");

            var users = await this._userRepository.Get();
            var userDtos = users.Select(u => u.ToDto()).ToList();

            return userDtos;
        }
    }
}
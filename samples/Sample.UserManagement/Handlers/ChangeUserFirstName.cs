using System;
using System.Threading.Tasks;
using Sample.UserManagement.Contract;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService
    {
        public async Task<UserDto> ChangeUserFirstName(Guid aggregateId, string firstName)
        {
            var user = await this._userRepository.Get(aggregateId);
            if (user == null)
            {
                throw new Exception($"User with ID {aggregateId} not found");
            }

            user.ChangeFirstName(firstName);

            await this._userRepository.Save(user);
            return user.ToDto();
        }
    }
}

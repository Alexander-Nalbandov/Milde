using System;
using System.Threading.Tasks;
using Sample.UserManagement.Contract;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService
    {
        public async Task<UserDto> ChangeUserAge(Guid aggregateId, int age)
        {
            var user = await this._userRepository.Get(aggregateId);
            if (user == null)
            {
                throw new Exception($"User with ID {aggregateId} not found");
            }

            user.ChangeAge(age);

            await this._userRepository.Save(user);
            return user.ToDto();
        }
    }
}

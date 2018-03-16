using System;
using System.Threading.Tasks;
using Sample.UserManagement.Contract;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService
    {
        public Task<UserDto> ChangeUserFirstName(Guid aggregateId, string firstName)
        {
            throw new NotImplementedException();
        }
    }
}

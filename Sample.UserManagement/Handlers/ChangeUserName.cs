using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sample.UserManagement.Contract;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService
    {
        public Task<UserDto> ChangeUserName(Guid aggregateId, string name)
        {
            throw new NotImplementedException();
        }
    }
}

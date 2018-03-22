using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sample.UserManagement.Contract;

namespace Sample.UserManagement.Handlers
{
    public partial class UserManagementService
    {
        public async Task<UserDto> ThrowException(Guid aggregateId)
        {
            throw new NotImplementedException();
        }
    }
}

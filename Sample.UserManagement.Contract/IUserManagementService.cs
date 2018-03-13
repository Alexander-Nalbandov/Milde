using System;
using System.Threading.Tasks;

namespace Sample.UserManagement.Contract
{
    public interface IUserManagementService
    {
        Task<UserDto> CreateUser(string name);
        Task<UserDto> ChangeUserName(Guid aggregateId, string name);
    }
}
using System;

namespace Sample.UserManagement.Contract
{
    public interface IUserManagementService
    {
        void CreateUser(string name);
        void ChangeUserName(Guid aggregateId, string name);
    }
}
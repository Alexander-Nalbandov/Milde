using AutoMapper;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Aggregates;

namespace Sample.UserManagement
{
    internal static class TypeExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return Mapper.Map<UserDto>(user);
        }
    }
}

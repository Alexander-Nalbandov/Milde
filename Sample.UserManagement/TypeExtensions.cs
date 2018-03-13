using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain;

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

using System;
using AutoMapper;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain;

namespace Sample.ConsoleClient
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Hello World!");
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());

            new SampleClientProgram().Run();
        }
    }
}
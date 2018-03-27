using System;
using AutoMapper;
using Sample.UserManagement.Contract;
using Sample.UserManagement.Domain.Aggregates;

namespace Sample.ConsoleClient
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Client has started.");
            Mapper.Initialize(cfg => cfg.CreateMap<User, UserDto>());

            new SampleClientProgram().Run();
        }
    }
}
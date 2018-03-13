using System;
using Sample.Infrastructure.Remoting.Rabbit;

namespace Sample.ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            new SampleClientProgram().Run();
        }
    }
}
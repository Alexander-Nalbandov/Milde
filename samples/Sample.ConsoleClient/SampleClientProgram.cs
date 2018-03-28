using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using Sample.UserManagement.Contract;

namespace Sample.ConsoleClient
{
    internal class SampleClientProgram
    {
        public async void Run(IContainer container)
        {
            var service = container.Resolve<IUserManagementService>();
            while (true)
            {
                Console.WriteLine("Command:");
                var command = Console.ReadLine();
                switch (command?.ToLowerInvariant())
                {
                    case "list":
                        await this.ListCommand(service);
                        break;
                    case "create":
                        await this.CreateCommand(service);
                        break;
                    case "populate":
                        await this.PopulateData(service);
                        break;
                    case "throw":
                        await service.ThrowException(Guid.Empty);
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }


        private async Task ListCommand(IUserManagementService service)
        {
            var users = await service.Get();
            if (!users.Any())
            {
                Console.WriteLine("There are no users.");
                return;
            }

            foreach (var user in users)
            {
                Console.WriteLine(JsonConvert.SerializeObject(user));
            }

            Console.WriteLine($"Total: {users.Count} users");
        }

        private async Task CreateCommand(IUserManagementService service)
        {
            Console.WriteLine("First Name:");
            var firstName = Console.ReadLine();

            Console.WriteLine("Last Name:");
            var lastName = Console.ReadLine();

            Console.WriteLine("Age:");
            var age = int.Parse(Console.ReadLine());

            var a = await service.CreateUser(firstName, lastName, age);
            Console.WriteLine($"Created {JsonConvert.SerializeObject(a)}");
        }

        private Task PopulateData(IUserManagementService service)
        {
            Console.WriteLine("Quantity:");
            var quantityString = Console.ReadLine();
            if (!int.TryParse(quantityString, out int quantity))
            {
                return Task.CompletedTask;
            }

            var rand = new Random();

            Parallel.ForEach(
                source: Enumerable.Range(0, quantity),
                parallelOptions: new ParallelOptions { MaxDegreeOfParallelism = 10 },
                body: async index =>
                {
                    var user = await service.CreateUser("A", "1", 18);
                    await service.ChangeUserFirstName(user.Id, "B");
                    await service.ChangeUserFirstName(user.Id, "User");
                    await service.ChangeUserLastName(user.Id, "2");
                    await service.ChangeUserLastName(user.Id, index.ToString());
                    await service.ChangeUserAge(user.Id, 20);
                    await service.ChangeUserAge(user.Id, rand.Next(20, 50));

                    Console.WriteLine($"{index} user was populated.");
                }
            );

            return Task.CompletedTask;
        }
    }
}
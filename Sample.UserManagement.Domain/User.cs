using System;
using Sample.Infrastructure.EventSourcing;

namespace Sample.UserManagement.Domain
{
    public class User : IAggregate
    {
        public Guid Id { get; private set; }
        public int ShardKey { get; private set; }
        public int Version { get; private set; }
        public string Name { get; private set; }

        public static User Create(string name)
        {
            return new User() {Id = Guid.NewGuid(), Name = name, Version = 1, ShardKey = 1};
        }

        public void ChangeName(string name)
        {
            Name = name;
            Version++;
        }
    }
}

using System;
using Milde.EventSourcing.Events;

namespace Sample.UserManagement.Contract.Events
{
    public class UserCreatedEvent : AggregateEvent
    {
        public UserCreatedEvent(
            Guid aggregateId, 
            int version, 
            int shardKey,
            string firstName, 
            string lastName, 
            int age
        ) 
            : base(aggregateId, version, shardKey)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public int Age { get; private set; }
    }
}

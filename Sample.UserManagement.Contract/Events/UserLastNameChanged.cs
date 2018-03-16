using System;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.UserManagement.Contract.Events
{
    public class UserLastNameChanged : AggregateEvent
    {
        public UserLastNameChanged(Guid aggregateId, int version, int shardKey, string lastName)
            : base(aggregateId, version, shardKey)
        {
            LastName = lastName;
        }

        public string LastName { get; private set; }
    }
}

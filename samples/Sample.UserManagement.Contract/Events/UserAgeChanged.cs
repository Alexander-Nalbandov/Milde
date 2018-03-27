using System;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.UserManagement.Contract.Events
{
    public class UserAgeChanged : AggregateEvent
    {
        public UserAgeChanged(Guid aggregateId, int version, int shardKey, int age)
            : base(aggregateId, version, shardKey)
        {
            Age = age;
        }

        public int Age { get; private set; }
    }
}

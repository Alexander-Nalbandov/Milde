using System;
using Milde.EventSourcing.Events;

namespace Sample.UserManagement.Contract.Events
{
    public class UserFirstNameChanged : AggregateEvent
    {
        public UserFirstNameChanged(Guid aggregateId, int version, int shardKey, string firstName) 
            : base(aggregateId, version, shardKey)
        {
            FirstName = firstName;
        }

        public string FirstName { get; private set; }
    }
}

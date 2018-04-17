using System;
using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Events;
using Newtonsoft.Json;
using Sample.UserManagement.Contract.Events;

namespace Sample.UserManagement.Domain.Aggregates
{
    public class User : Aggregate
    {
        public User(Guid id) : base(id)
        {
        }

        // TODO: Remove and make proper deserialization
        [JsonConstructor]
        public User(Guid id, int version, int shardKey, string firstName, string lastName, int age)
            : base(id, version, shardKey)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
        

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public int Age { get; private set; }


        public static User Create(string firstName, string lastName, int age)
        {
            var user = new User(Guid.NewGuid());

            user.ApplyEvent(
                new UserCreatedEvent(user.Id, user.Version + 1, user.ShardKey, firstName, lastName, age)
            );

            return user;
        }


        public void ChangeFirstName(string firstName)
        {
            if (this.FirstName != firstName)
            {
                this.ApplyEvent(
                    new UserFirstNameChanged(this.Id, this.Version + 1, this.ShardKey, firstName)
                );
            }
        }

        public void ChangeLastName(string lastName)
        {
            if (this.LastName != lastName)
            {
                this.ApplyEvent(
                    new UserLastNameChanged(this.Id, this.Version + 1, this.ShardKey, lastName)
                );
            }
        }

        public void ChangeAge(int age)
        {
            if (this.Age != age)
            {
                this.ApplyEvent(
                    new UserAgeChanged(this.Id, this.Version + 1, this.ShardKey, age)
                );
            }
        }


        private void Apply(UserCreatedEvent @event)
        {
            this.FirstName = @event.FirstName;
            this.LastName = @event.LastName;
            this.Age = @event.Age;
        }

        private void Apply(UserFirstNameChanged @event)
        {
            this.FirstName = @event.FirstName;
        }

        private void Apply(UserLastNameChanged @event)
        {
            this.LastName = @event.LastName;
        }

        private void Apply(UserAgeChanged @event)
        {
            this.Age = @event.Age;
        }

        protected override void CallApply(IAggregateEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
        }
    }
}

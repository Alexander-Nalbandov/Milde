using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Cache;
using Milde.EventSourcing.Events;

namespace Sample.UserManagement.Domain.Repositories
{
    public class UserRepository : AggregateRepository<Aggregates.User>, IUserRepository
    {
        public UserRepository(IEventStore eventStore, IAggregateCache<Aggregates.User> cache) 
            : base(eventStore, cache)
        {
        }
    }
}

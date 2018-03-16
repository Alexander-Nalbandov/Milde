using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Cache;
using Sample.Infrastructure.EventSourcing.Events;

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

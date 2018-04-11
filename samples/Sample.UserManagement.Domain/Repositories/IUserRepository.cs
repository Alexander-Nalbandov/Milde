using Milde.EventSourcing.Aggregates;

namespace Sample.UserManagement.Domain.Repositories
{
    public interface IUserRepository : IAggregateRepository<Aggregates.User>
    {
    }
}

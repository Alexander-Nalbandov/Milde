using System.Threading.Tasks;
using Sample.Infrastructure.EventSourcing.Events;

namespace Sample.Infrastructure.EventSourcing.EventBus
{
    public interface IEventHandler<TEvent>
        where TEvent : class, IAggregateEvent
    {
        Task Handle(TEvent @event);
    }
}
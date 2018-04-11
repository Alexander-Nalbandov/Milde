using System.Threading.Tasks;
using Milde.EventSourcing.Events;

namespace Milde.EventSourcing.EventBus
{
    public interface IEventHandler<TEvent>
        where TEvent : class, IAggregateEvent
    {
        Task Handle(TEvent @event);
    }
}
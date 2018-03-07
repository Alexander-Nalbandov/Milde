namespace Sample.Infrastructure.EventSourcing
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milde.EventSourcing.Events;

namespace Milde.EventSourcing.EventBus
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent;
        Task Publish<TEvent>(IList<TEvent> events) where TEvent : class, IAggregateEvent;
        Task Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IAggregateEvent;
        Task Subscribe(Type eventType, Func<IAggregateEvent, Task> handler);
    }
}

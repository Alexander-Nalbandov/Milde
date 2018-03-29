using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sample.Infrastructure.EventSourcing.EventBus;
using Sample.Infrastructure.EventSourcing.Events;
using Sample.UserManagement.Contract.Events;
using Serilog;

namespace Sample.ConsoleClient.Handlers
{
    public class UserEventsHandler : IEventHandler<UserCreatedEvent>, 
                                     IEventHandler<UserFirstNameChanged>,
                                     IEventHandler<UserLastNameChanged>, 
                                     IEventHandler<UserAgeChanged>
    {
        private readonly ILogger _logger;


        public UserEventsHandler(ILogger logger)
        {
            _logger = logger;
        }


        public Task Handle(UserCreatedEvent @event)
        {
            return this.HandleEvent(@event);
        }

        public Task Handle(UserFirstNameChanged @event)
        {
            return this.HandleEvent(@event);
        }

        public Task Handle(UserLastNameChanged @event)
        {
            return this.HandleEvent(@event);
        }

        public Task Handle(UserAgeChanged @event)
        {
            return this.HandleEvent(@event);
        }


        private Task HandleEvent(IAggregateEvent @event)
        {
            this._logger.Information("{EventType} event was handled from EventBus", @event.GetType().Name);
            return Task.CompletedTask;
        }
    }
}

using System.Linq;
using Autofac;

namespace Sample.Infrastructure.EventSourcing.EventBus
{
    public static class EventBusModule
    {
        public static ContainerBuilder RegisterEventHandlers(this ContainerBuilder builder)
        {
            builder.RegisterBuildCallback(container =>
            {
                var eventBus = container.Resolve<IEventBus>();
                var eventHandlers = (
                    from registration in container.ComponentRegistry.Registrations
                    from @interface in registration.Activator.LimitType.GetInterfaces()
                    where @interface.IsGenericType &&
                          @interface.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    select new
                    {
                        EventType = @interface.GenericTypeArguments.First(),
                        Handler = registration
                    }
                );

                foreach (var handler in eventHandlers)
                {
                    var eventHandler = (dynamic)container.Resolve(handler.Handler.Activator.LimitType);
                    eventBus.Subscribe(handler.EventType, @event => eventHandler.Handle((dynamic) @event));
                }
            });

            return builder;
        }
    }
}

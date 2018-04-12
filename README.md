# Milde
Milde helps building distributed applications easier providing RPC and ES functinality.
Currently RPC only supports Rabbit transport but any other PUB/SUP transport can be easily implemented.
ES uses EventStore as event store and Redis as state cache
In addition Milde provides HostBuilder standartizing startup setup. 
If you have your own HostBuilder implemented, Milde's functionality can be wired to Autofac container.

## NuGet Packages
| Name | Description |
|------|:------------|
| Milde.Remoting | RPC implementation library. Contains client and sever classes |
| Milde.Remoting.Rabbit | Rabbit transport implementation for remoting and RPC registration extensions|
| Milde.EventSourcing | ES abstractions |
| Milde.EventStore | EventBus and EventStore implementation |
| Milde.Redis | ES Aggregate state cache in Redis |
| Milde.AggregateCache | ES Aggregate state cache saving state both to Redis and in-memory |
| Milde.Hosting | Contains host builder to do startup app initialization |

## Infrastructure requirements
To simplify local testing, Milde comes with a [docker-compose](https://github.com/Alexander-Nalbandov/Milde/blob/master/docker-compose.yml) description of all the required infrastructure components.
Comment out what you don't need and just run 

```cs
docker-compose up
```

## RPC
`Milde.Infrastructure.Remoting` cannot be used by itself, it requires some transport implementation. 
`Milde.Infrastructure.Remoting.Rabbit` provides this transport implementation. 
Basic idea of this remoting is that we can call remote service the same way we would call normal c# method.
However, there are some restrictions on the remote contract - methods have to be async and have to return some result. 
Here's the example of how the remote contract could look like:

```cs
public interface IUserManagementService
{
    Task<IList<UserDto>> Get();
    Task<UserDto> CreateUser(string firstName, string lastName, long age);
    Task<UserDto> ChangeUserFirstName(Guid aggregateId, string firstName);
    Task<UserDto> ChangeUserLastName(Guid aggregateId, string lastName);
    Task<UserDto> ChangeUserAge(Guid aggregateId, int age);
}
```

### Rabbit configuration

By default rabbit configration is read from .Net Core `IConfiguration` rabbit section.
Following keys have to be present in configuration: rabbit:address, rabbit:username, rabbit:password, rabbit:vhost.

### Client registration

To register service on the client, call WithRabbitRemoting on a HostBuilder or Autofac's ContainerBuilder and pass service registration calls.
Example (HostBuilder):

```cs
 var host = HostBuilder
                       .Create()
                       .WithConfiguration(configBuilder =>
                       {
                           configBuilder.AddCommandLine();
                       })
                       .WithLogger(loggerConfiguration => loggerConfiguration.WriteTo.Console())
                       .WithRabbitRemoting(configurator =>
                       {
                           configurator.RegisterProxy<IUserManagementService>();
                       })
                       .Build();
host.Run((container, configuration) => {});
```
Same approach applies to `ContainerBuilder` extension. Both approaches require Serilog's `ILogger` and .Net Core `ConfigurationRoot` (as `IConfiguration`) registered in container. 

### Server registration

To register service implementation, call `WithRabbitRemoting` just like we did in client registration.
In configuration action, call configurator's `RegisterService` passing service interface and implementation in type parameters.

With HostBuilder, you can have different services hosted in a single process. 
These different services can use different RPC/EventBus/EventStore. 
To register such services, implement `IService` and register it with `HostBuilder.WithService()` and register transport & persistence in `IService.RegisterDependencies()`.

## Event sourcing

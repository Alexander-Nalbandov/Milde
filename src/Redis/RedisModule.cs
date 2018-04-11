using Autofac;
using Milde.EventSourcing.Hosting;
using Milde.Redis.AggregateCache;
using StackExchange.Redis;

namespace Milde.Redis
{
    public static class RedisModule
    {
        public static IEventSourcingConfiguration WithRedisAggregateCache(this IEventSourcingConfiguration eventSourcingConfiguration)
        {
            return eventSourcingConfiguration.WithAggregateCache(
                typeof(RedisAggregateCache<>),
                (builder, config) =>
                {
                    var connectionString = config["Redis:ConnectionString"];
                    var redis = ConnectionMultiplexer.Connect(connectionString);
                    var database = redis.GetDatabase(0);

                    builder.RegisterGeneric(typeof(RedisAggregateCache<>)).As(typeof(IRedisAggregateCache<>)).SingleInstance();
                    builder.RegisterInstance(database).AsImplementedInterfaces().SingleInstance();
                }
            );
        }
    }
}

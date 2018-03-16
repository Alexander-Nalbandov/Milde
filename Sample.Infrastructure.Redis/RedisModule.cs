using Autofac;
using Sample.Infrastructure.Redis.AggregateCache;
using StackExchange.Redis;

namespace Sample.Infrastructure.Redis
{
    public static class RedisModule
    {
        public static ContainerBuilder UseRedis(this ContainerBuilder builder, string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            var database = redis.GetDatabase(0);

            builder.RegisterInstance(database).AsImplementedInterfaces().SingleInstance();

            return builder;
        }

        public static ContainerBuilder UseRedisAggregateCache(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RedisAggregateCache<>)).As(typeof(IRedisAggregateCache<>)).SingleInstance();

            return builder;
        }
    }
}

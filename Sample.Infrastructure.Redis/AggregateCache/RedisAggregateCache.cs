using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Events;
using Sample.Infrastructure.EventSourcing.Serialization;
using StackExchange.Redis;

namespace Sample.Infrastructure.Redis.AggregateCache
{
    class RedisAggregateCache<TAggregate> : IRedisAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        private readonly IDatabase _redis;
        private readonly ISerializer _serializer;
        


        public RedisAggregateCache(IDatabase redis, ISerializer serializer)
        {
            _redis = redis;
            _serializer = serializer;
        }


        public async Task Initialize(IEventStore eventStore)
        {
            // TODO: Optimize
            var events = await eventStore.GetAggregateEvents<TAggregate>();
            var eventsByAggregate = events.GroupBy(e => e.AggregateId);

            var aggregates = await this.GetAllAggregates();

            foreach (var aggregateEvents in eventsByAggregate)
            {
                var aggregate = aggregates.SingleOrDefault(a => a.Id == aggregateEvents.Key) ??
                                Activator.CreateInstance(typeof(TAggregate), aggregateEvents.Key) as TAggregate;

                var eventsToApply = aggregateEvents.Where(e => e.Version >= aggregate.Version).OrderBy(e => e.Version);

                foreach (var @event in eventsToApply)
                {
                    aggregate.ApplyEvent(@event);
                }

                await this.SaveAggregate(aggregate);
            }
        }


        public async Task<IList<TAggregate>> GetAllAggregates()
        {
            var key = this.GetKey();
            var values = await this._redis.HashGetAllAsync(key);
            return values.Select(v => this._serializer.Deserialize<TAggregate>(v.Value)).ToList();
        }

        public async Task<TAggregate> GetAggregate(Guid id)
        {
            var key = this.GetKey();
            var hashField = id.ToString();
            var value = await this._redis.HashGetAsync(key, hashField);
            return this._serializer.Deserialize<TAggregate>(value);
        }

        public Task SaveAggregate(TAggregate aggregate)
        {
            var key = this.GetKey();
            var hashField = aggregate.Id.ToString();
            var hashValue = this._serializer.Serialize(aggregate);
            return this._redis.HashSetAsync(key, hashField, hashValue);
        }


        private RedisKey GetKey()
        {
            var key = typeof(TAggregate).Name;
            return key;
        }
    }
}

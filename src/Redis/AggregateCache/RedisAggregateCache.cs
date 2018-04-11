using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milde.EventSourcing.Aggregates;
using Milde.EventSourcing.Events;
using Milde.EventSourcing.Serialization;
using Serilog;
using StackExchange.Redis;

namespace Milde.Redis.AggregateCache
{
    class RedisAggregateCache<TAggregate> : IRedisAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        private readonly IDatabase _redis;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        
        
        public RedisAggregateCache(IDatabase redis, ISerializer serializer, ILogger logger)
        {
            _redis = redis;
            _serializer = serializer;
            _logger = logger;
        }


        public async Task Initialize(IEventStore eventStore)
        {
            var existingAggregates = await this.GetAllAggregates();
            var aggregatesToSave = await eventStore.UpdateAggregates(existingAggregates);

            await this.SaveAggregates(aggregatesToSave);

            this._logger.Information("{AggregateType} are restored to Redis cache.", typeof(TAggregate).Name);
        }

        //public async Task Initialize(IEventStore eventStore)
        //{
        //    var existingAggregates = await this.GetAllAggregates();
        //    var events = await eventStore.GetAggregateEvents<TAggregate>();
            
        //    var eventsByAggregate = events.GroupBy(e => e.AggregateId);
        //    var aggregatesToSave = new List<TAggregate>();
        //    foreach (var aggregateEvents in eventsByAggregate)
        //    {
        //        var aggregate = existingAggregates.SingleOrDefault(a => a.Id == aggregateEvents.Key) ??
        //                        (TAggregate) Activator.CreateInstance(typeof(TAggregate), aggregateEvents.Key);

        //        var eventsToApply = aggregateEvents.Where(e => e.Version >= aggregate.Version).OrderBy(e => e.Version);

        //        foreach (var @event in eventsToApply)
        //        {
        //            aggregate.ApplyEvent(@event);
        //        }

        //        aggregatesToSave.Add(aggregate);
        //    }
            
        //    await this.SaveAggregates(aggregatesToSave);

        //    this._logger.Information("{AggregateType} are restored to Redis cache.", typeof(TAggregate).Name);
        //}


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
            return this.SaveAggregates(new[] {aggregate});
        }

        public Task SaveAggregates(IList<TAggregate> aggregates)
        {
            var key = this.GetKey();
            var hashEntries = aggregates.Select(aggregate =>
            {
                var hashField = aggregate.Id.ToString();
                var hashValue = this._serializer.Serialize(aggregate);
                return new HashEntry(hashField, hashValue);
            }).ToArray();

            return this._redis.HashSetAsync(key, hashEntries);
        }


        private RedisKey GetKey()
        {
            var key = typeof(TAggregate).Name;
            return key;
        }
    }
}

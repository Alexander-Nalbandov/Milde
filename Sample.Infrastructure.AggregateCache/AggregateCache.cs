using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample.Infrastructure.AggregateCache.InMemory;
using Sample.Infrastructure.EventSourcing.Aggregates;
using Sample.Infrastructure.EventSourcing.Cache;
using Sample.Infrastructure.EventSourcing.Events;
using Sample.Infrastructure.Redis.AggregateCache;

namespace Sample.Infrastructure.AggregateCache
{
    class AggregateCache<TAggregate> : IAggregateCache<TAggregate>
        where TAggregate : class, IAggregate
    {
        private readonly IRedisAggregateCache<TAggregate> _redisCache;
        private readonly IInMemoryAggregateCache<TAggregate> _inMemoryCache;


        public AggregateCache(IRedisAggregateCache<TAggregate> redisCache, IInMemoryAggregateCache<TAggregate> inMemoryCache)
        {
            _redisCache = redisCache;
            _inMemoryCache = inMemoryCache;
        }


        public async Task Initialize(IEventStore eventStore)
        {
            await this._redisCache.Initialize(eventStore);

            var aggregates = await this._redisCache.GetAllAggregates();
            this._inMemoryCache.PopulateCache(aggregates);
        }


        public Task<IList<TAggregate>> GetAllAggregates()
        {
            return this._inMemoryCache.GetAllAggregates();
        }

        public Task<TAggregate> GetAggregate(Guid id)
        {
            return this._inMemoryCache.GetAggregate(id);
        }

        public async Task SaveAggregate(TAggregate aggregate)
        {
            await this._redisCache.SaveAggregate(aggregate);
            await this._inMemoryCache.SaveAggregate(aggregate);
        }
    }
}

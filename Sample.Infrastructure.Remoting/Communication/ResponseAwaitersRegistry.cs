using System.Collections.Concurrent;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public class ResponseAwaitersRegistry<TResponse>
        where TResponse : IRemoteMessage
    {
        private readonly ConcurrentDictionary<string, ResponseAwaiter<TResponse>> _responseAwaiters;


        public ResponseAwaitersRegistry()
        {
            this._responseAwaiters = new ConcurrentDictionary<string, ResponseAwaiter<TResponse>>();
        }


        public ResponseAwaiter<TResponse> Register()
        {
            var awaiter = new ResponseAwaiter<TResponse>();
            this._responseAwaiters.TryAdd(awaiter.CorrelationId, awaiter);

            return awaiter;
        }

        public ResponseAwaiter<TResponse> Get(string correlationId)
        {
            if (this._responseAwaiters.TryGetValue(correlationId, out ResponseAwaiter<TResponse> responseAwaiter))
            {
                return responseAwaiter;
            }

            return null;
        }

        public void Remove(ResponseAwaiter<TResponse> awaiter)
        {
            this._responseAwaiters.TryRemove(awaiter.CorrelationId, out ResponseAwaiter<TResponse> _);
        }
    }
}
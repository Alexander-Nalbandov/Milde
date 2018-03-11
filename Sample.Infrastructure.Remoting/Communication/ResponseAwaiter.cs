using System;
using System.Threading.Tasks;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    internal class ResponseAwaiter<TResponse>
        where TResponse : IRemoteMessage
    {
        private readonly TaskCompletionSource<TResponse> _completionSource;



        public ResponseAwaiter()
        {
            this.CorrelationId = Guid.NewGuid().ToString();
            this._completionSource = new TaskCompletionSource<TResponse>();
        }



        public string CorrelationId { get; }



        public void SetResult(TResponse result)
        {
            this._completionSource.SetResult(result);
        }

        public void SetException(Exception ex)
        {
            this._completionSource.SetException(ex);
        }

        public Task<TResponse> Result => this._completionSource.Task;
    }
}
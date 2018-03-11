using System;
using System.Threading.Tasks;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    internal class RemoteProcedureExecutor<TRequest, TResponse>
        where TRequest : IRemoteMessage
        where TResponse : IRemoteMessage
    {
        private readonly ResponseAwaitersRegistry<TResponse> _awaitersRegistry;
        private readonly ISender<TRequest> _sender;
        private readonly IListener<TResponse> _responseListener;

        public RemoteProcedureExecutor(ResponseAwaitersRegistry<TResponse> awaitersRegistry, ISender<TRequest> sender, IListener<TResponse> responseListener)
        {
            _awaitersRegistry = awaitersRegistry;
            _sender = sender;
            _responseListener = responseListener;
            this._responseListener.AddHandler(this.HandleResponse);
        }

        public async Task<TResponse> Execute(TRequest request, string routingKey)
        {
            var awaiter = this._awaitersRegistry.Register();
            request.Headers.CorrelationId = awaiter.CorrelationId;
            request.Headers.RoutingKey = routingKey;
            request.Headers.ReplyTo = ""; //TODO do we need it here?
            _sender.Send(request);
            var response = await awaiter.Result;
            return response;
        }

        private bool HandleResponse(TResponse response)
        {
            var responseAwaiter = this._awaitersRegistry.Get(response.Headers.CorrelationId);
            if (responseAwaiter == null)
            {
                return false;
            }
            try
            {
                responseAwaiter.SetResult(response);
            }
            catch (Exception ex)
            {
                responseAwaiter.SetException(ex);
            }
            finally
            {
                this._awaitersRegistry.Remove(responseAwaiter);
            }

            return true;
        }
    }
}
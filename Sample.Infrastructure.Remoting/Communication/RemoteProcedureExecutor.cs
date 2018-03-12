using System;
using System.Threading.Tasks;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public class RemoteProcedureExecutor<TInterface>
    {
        private readonly ResponseAwaitersRegistry<RemoteResponse> _awaitersRegistry;
        private readonly ISender<TInterface> _sender;
        private readonly IListener<TInterface> _responseListener;

        public RemoteProcedureExecutor(ResponseAwaitersRegistry<RemoteResponse> awaitersRegistry, ISender<TInterface> sender, IListener<TInterface> responseListener)
        {
            _awaitersRegistry = awaitersRegistry;
            _sender = sender;
            _responseListener = responseListener;
            this._responseListener.AddHandler(this.HandleResponse);
        }

        public async Task<RemoteResponse> Execute(RemoteRequest request, string routingKey)
        {
            var awaiter = this._awaitersRegistry.Register();
            request.Headers.CorrelationId = awaiter.CorrelationId;
            request.Headers.RoutingKey = routingKey;
            request.Headers.ReplyTo = ""; //TODO do we need it here?
            _sender.Send(request);
            var response = await awaiter.Result;
            return response;
        }

        private bool HandleResponse(RemoteResponse response)
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
using System;
using System.Threading.Tasks;
using Autofac;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Service
{
    internal class MessageHandler<TImplementation, TInterface> : IStartable where TImplementation : TInterface
    {
        private readonly TImplementation _instance;
        private readonly IListener<TInterface, RemoteRequest> _listener;
        private readonly ISender<TInterface, RemoteResponse> _sender;

        public MessageHandler(IListener<TInterface, RemoteRequest> listener, ISender<TInterface, RemoteResponse> sender,
            TImplementation instance)
        {
            _instance = instance;
            _listener = listener;
            _sender = sender;
            _listener.AddHandler(HandleRequest);
        }

        public void Start()
        {
        }

        private bool HandleRequest(RemoteRequest request)
        {
            var response = this.GetResponse(request.MethodName, request.Args);

            this.SendResponse(response, request.Headers);

            return true;
        }

        private object GetResponse(string methodName, object[] args)
        {
            var method = typeof(TInterface).GetMethod(methodName);

            if (method == null)
            {
                throw new MissingMethodException(typeof(TInterface).Name, methodName);
            }

            if (!typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                throw new InvalidOperationException("Message handler supports only asynchronous methods.");
            }

            if (!method.ReturnType.IsGenericType)
            {
                throw new InvalidOperationException("Message handler doesn't support VOID methods.");
            }

            var result = method.Invoke(_instance, args);
            var asyncResult = ((Task)result).GetType().GetProperty("Result")?.GetValue(result);
            return asyncResult;
        }

        private void SendResponse(object response, MessageHeaders requestHeaders)
        {
            var msg = new RemoteResponse(response)
            {
                Headers = { CorrelationId = requestHeaders.CorrelationId, RoutingKey = requestHeaders.RoutingKey}
            };
            _sender.Send(msg);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Remoting
{
    class ServiceProxy<TInterface> : DispatchProxy
    {
        private readonly RemoteProcedureExecutor _executor;

        public ServiceProxy(RemoteProcedureExecutor executor)
        {
            _executor = executor;
        }

        protected override object Invoke(MethodInfo method, object[] args)
        {
            if (!typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                throw new InvalidOperationException("ServiceProxy supports only asynchronous methods.");
            }

            if (!method.ReturnType.IsGenericType)
            {
                throw new InvalidOperationException("ServiceProxy doesn't support VOID methods.");
            }

            var command = new RemoteRequest(method.Name, args);
        }

        private string GetRoutingKey(string methodName)
        {
            return $"{this._client.Segments.Last()}.{methodName}";
        }
    }

    internal class RemoteProcedureExecutor<TRequest, TResponse>
        where TRequest : IRemoteMessage
        where TResponse : IRemoteMessage
    {

    }

    internal interface IListener<TMessage> where TMessage : IRemoteMessage
    {
        void StartPolling();
        void AddHandler();
    }

    internal interface ISender<TMessage> where TMessage : IRemoteMessage
    {
        void Send(TMessage message, string routingKey);
    }
}

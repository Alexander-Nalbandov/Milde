using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Milde.Remoting.Communication;
using Newtonsoft.Json;

namespace Milde.Remoting.Client
{
    public class ServiceProxy<TInterface> : DispatchProxy
    {
        private ResponseConverter _converter;
        private RemoteProcedureExecutor<TInterface> _executor;

        private void SetExecutor(RemoteProcedureExecutor<TInterface> executor)
        {
            this._executor = executor;
        }

        private void SetConverter(ResponseConverter converter)
        {
            this._converter = converter;
        }

        internal static TInterface Create(RemoteProcedureExecutor<TInterface> executor, ResponseConverter converter)
        {
            object proxy = Create<TInterface, ServiceProxy<TInterface>>();
            ((ServiceProxy<TInterface>) proxy).SetExecutor(executor);
            ((ServiceProxy<TInterface>) proxy).SetConverter(converter);
            return (TInterface) proxy;
        }

        protected override dynamic Invoke(MethodInfo method, object[] args)
        {
            if (!typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                throw new InvalidOperationException("ServiceProxy supports only asynchronous methods.");
            }

            if (!method.ReturnType.IsGenericType)
            {
                throw new InvalidOperationException("ServiceProxy doesn't support VOID methods.");
            }

            var request = new RemoteRequest(method.Name, args.Select(JsonConvert.SerializeObject).ToArray());
            var routingKey = GetRoutingKey(method.Name);
            var response = this._executor.Execute(request, routingKey).Result;

            var convertedResponse = this._converter.Convert(response, method.ReturnType.GetGenericArguments().First());
            return convertedResponse;
        }

        private string GetRoutingKey(string methodName)
        {
            return $"{typeof(TInterface).Name}.{methodName}";
        }
    }
}
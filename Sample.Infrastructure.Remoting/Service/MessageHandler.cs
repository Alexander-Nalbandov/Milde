using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Serilog;

namespace Sample.Infrastructure.Remoting.Service
{
    internal class MessageHandler<TImplementation, TInterface> : IStartable where TImplementation : TInterface
    {
        private readonly TImplementation _instance;
        private readonly ILogger _logger;
        private readonly IListener<TInterface, RemoteRequest> _listener;
        private readonly ISender<TInterface, RemoteResponse> _sender;

        public MessageHandler(IListener<TInterface, RemoteRequest> listener, ISender<TInterface, RemoteResponse> sender,
            TImplementation instance, ILogger logger)
        {
            _logger = logger;
            _instance = instance;
            _listener = listener;
            _sender = sender;
            _listener.AddHandler(HandleRequest);
        }

        public void Start()
        {
            _logger.Information("Service {ServiceName} has started", typeof(TInterface).Name);
        }

        private bool HandleRequest(RemoteRequest request)
        {
            _logger.Debug("{ServiceName}: Handling {RequestName}", typeof(TInterface).Name, request.MethodName);
            try
            {
                var response = this.GetResponse(request.MethodName, request.Args);
                this.SendResponse(response, request.Headers);
            }
            catch (TargetInvocationException e) when (e.InnerException is AggregateException)
            {
                this.SendErrorResponse(((AggregateException)e.InnerException).InnerExceptions.First(), request.Headers);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Sad thing occured");
                return false;
            }

            return true;
        }

        private object GetResponse(string methodName, string[] args)
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

            var parameters = method.GetParameters();
            var typedParameters = new object[parameters.Length];
            foreach (var cnt in Enumerable.Range(0, parameters.Length))
            {
                typedParameters[cnt] = 
                    JsonConvert.DeserializeObject(args[cnt].ToString(), parameters[cnt].ParameterType);
            }

            var result = method.Invoke(_instance, typedParameters);
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

        private void SendErrorResponse(Exception exception, MessageHeaders requestHeaders)
        {
            var msg = new RemoteResponse(exception)
            {
                Headers = { CorrelationId = requestHeaders.CorrelationId, RoutingKey = requestHeaders.RoutingKey }
            };
            _sender.Send(msg);
        }
    }
}
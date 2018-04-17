using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Milde.Remoting.Communication;
using Milde.Remoting.Contracts;
using Newtonsoft.Json;
using Serilog;

namespace Milde.Remoting.Service
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
            _logger.Information("Service {ServiceName} has started", typeof(TImplementation).Name);
        }

        private bool HandleRequest(RemoteRequest request)
        {
            _logger.Debug("{ServiceName}: Handling {RequestName}", typeof(TImplementation).Name, request.MethodName);
            try
            {
                var response = this.GetResponse(request.MethodName, request.Args);
                this.SendResponse(response, request.Headers);
            }
            catch (TargetInvocationException e) when (e.InnerException is AggregateException)
            {
                var sourceException = ((AggregateException) e.InnerException).InnerExceptions.First();
                _logger.Warning(sourceException, "{ServiceName}: Exception at {RequestName}", typeof(TImplementation).Name, request.MethodName);
                this.SendErrorResponse(sourceException, request.Headers);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Sad thing occured");
                this.SendErrorResponse(e, request.Headers);
                return true;
            }

            return true;
        }

        //TODO support versioining for services
        //Outdated client should have the ability to call newer server with no issue
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

            if (parameters.Length != args.Length)
            {
                throw new ArgumentException($"Unexpected number of agruments in call to {methodName}. Exected {parameters.Length}, received {args.Length}");
            }

            var typedParameters = new object[parameters.Length];
            foreach (var cnt in Enumerable.Range(0, parameters.Length))
            {
                try
                {
                    //TODO: do more strict typing. Currently value 123 can be assigned to bool
                    typedParameters[cnt] =
                        JsonConvert.DeserializeObject(args[cnt].ToString(), parameters[cnt].ParameterType);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Unable to cast value {args[cnt]} of argument {parameters[cnt].Name} to type {parameters[cnt].ParameterType} in method {methodName}");
                }
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
using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Sample.Infrastructure.Remoting.Communication;

namespace Sample.Infrastructure.Remoting.Service
{
    public class MessageHandler<TImplementation, TInterface> : IStartable where TImplementation : TInterface
    {
        private readonly IListener<TInterface, RemoteRequest> _listener;
        private readonly ISender<TInterface, RemoteResponse> _sender;
        private readonly TImplementation _instance;

        public MessageHandler(IListener<TInterface, RemoteRequest> listener, ISender<TInterface, RemoteResponse> sender, TImplementation instance)
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
            var methodInfo = typeof(TInterface).GetMethod(request.MethodName);
            var instanse = Expression.Constant(_instance);
            ParameterExpression[] parameters = methodInfo.GetParameters().Select(e => Expression.Parameter(e.ParameterType)).ToArray();
            var methodCallExpression = Expression.Call(instanse, methodInfo, parameters);
            var funcExpr = Expression.Lambda<Func<object[], dynamic>>(methodCallExpression, parameters).Compile();
            var result = funcExpr(request.Args);
            return true;
        }
    }
}
using System;
using Autofac;
using Moq;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Sample.Infrastructure.Remoting.Tests.Client;

namespace Sample.Infrastructure.Remoting.Tests
{
    internal static class Extensions
    {
        public static ContainerBuilder RegisterMockTransport<TInterface>(this ContainerBuilder builder, 
            Func<RemoteRequest, RemoteResponse> processAction)
        {
            Func<RemoteResponse, bool> handlerClosure = null;

            var sender = new Mock<ISender<ITestInterface, RemoteRequest>>();
            var listener = new Mock<IListener<ITestInterface, RemoteResponse>>();

            sender.Setup(s => s.Send(It.IsAny<RemoteRequest>()))
                .Callback<RemoteRequest>(req =>
                {
                    var response = processAction(req);
                    response.Headers.CorrelationId = req.Headers.CorrelationId;
                    handlerClosure(response);
                });

            listener.Setup(l => l.AddHandler(It.IsAny<Func<RemoteResponse, bool>>()))
                .Callback<Func<RemoteResponse, bool>>(func => handlerClosure = func);

            builder.RegisterInstance(sender.Object).AsImplementedInterfaces();
            builder.RegisterInstance(listener.Object).AsImplementedInterfaces();
            return builder;
        }
    }
}
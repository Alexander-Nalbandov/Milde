using System;
using Moq;
using Sample.Infrastructure.Remoting.Client;
using Sample.Infrastructure.Remoting.Communication;
using Sample.Infrastructure.Remoting.Contracts;
using Shouldly;
using Xunit;

namespace Sample.Infrastructure.Remoting.Tests
{
    public class ServiceProxyTest
    {
        [Fact]
        public void TestTrue()
        {
            true.ShouldBe(true);
        }

        [Fact]
        public void TestInvocation()
        {
            var registry = new ResponseAwaitersRegistry<RemoteResponse>();

            Func<IRemoteMessage, bool> handler = null;

            var sender = new Mock<ISender<IInterface, RemoteRequest>>();
            var listener = new Mock<IListener<IInterface, RemoteResponse>>();

            listener.Setup(l => l.AddHandler(It.IsAny<Func<IRemoteMessage, bool>>()))
                .Callback<Func<IRemoteMessage, bool>>((func) => handler = func);

            sender.Setup(s => s.Send(It.IsAny<RemoteRequest>()))
                .Callback<RemoteRequest>((req) => handler(new RemoteResponse(new object())));

            var executor = new RemoteProcedureExecutor<IInterface>(
                registry,
                sender.Object,
                listener.Object
            );
            var proxy = ServiceProxyFactory.Create<IInterface>(executor, new ResponseConverter());
        }

        private class Broker
        {
            private Func<IRemoteMessage, bool> _handler ;

            public void AddHandler(Func<IRemoteMessage, bool> handler)
            {
                _handler = handler;
            }

            public bool Invoke(IRemoteMessage msg)
            {
                return _handler(msg);
            }
        }

        private interface IInterface
        {
            
        }
    }
}
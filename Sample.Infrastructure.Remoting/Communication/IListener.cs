using System;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public interface IListener<TInterface, out TMessage> where TMessage : IRemoteMessage
    {
        void StartPolling();
        void AddHandler(Func<TMessage, bool> handler);
    }
}
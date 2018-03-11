using System;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public interface IListener<TMessage> where TMessage : IRemoteMessage
    {
        void StartPolling();
        void AddHandler(Func<TMessage, bool> handler);
    }
}
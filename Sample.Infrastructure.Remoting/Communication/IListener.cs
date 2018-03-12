using System;
using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public interface IListener<TInterface>
    {
        void StartPolling();
        void AddHandler(Func<RemoteResponse, bool> handler);
    }
}
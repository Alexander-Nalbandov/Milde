using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public interface ISender<TInterface, in TMessage> where TMessage : IRemoteMessage
    {
        void Send(TMessage message);
    }
}
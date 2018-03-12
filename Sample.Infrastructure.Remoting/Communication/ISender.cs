using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Communication
{
    public interface ISender<TInterface>
    {
        void Send(RemoteRequest message);
    }
}
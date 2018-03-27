using Sample.Infrastructure.Remoting.Contracts;

namespace Sample.Infrastructure.Remoting.Serialization
{
    public interface ISerializer
    {
        TMessage Deserialize<TMessage>(byte[] body) where TMessage : IRemoteMessage;
        byte[] Serialize<TMessage>(TMessage message) where TMessage : IRemoteMessage;
    }
}
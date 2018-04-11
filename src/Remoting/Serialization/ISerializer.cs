using Milde.Remoting.Contracts;

namespace Milde.Remoting.Serialization
{
    public interface ISerializer
    {
        TMessage Deserialize<TMessage>(byte[] body) where TMessage : IRemoteMessage;
        byte[] Serialize<TMessage>(TMessage message) where TMessage : IRemoteMessage;
    }
}
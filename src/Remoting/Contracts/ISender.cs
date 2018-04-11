namespace Milde.Remoting.Contracts
{
    public interface ISender<TInterface, in TMessage> where TMessage : IRemoteMessage
    {
        void Send(TMessage message);
    }
}
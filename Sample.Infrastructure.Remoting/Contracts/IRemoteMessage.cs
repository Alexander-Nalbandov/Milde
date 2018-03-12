namespace Sample.Infrastructure.Remoting.Contracts
{
    public interface IRemoteMessage
    {
        MessageHeaders Headers { get; }
    }
}
namespace Sample.Infrastructure.Remoting
{
    internal interface IRemoteMessage
    {
        MessageHeaders Headers { get; }
    }

    internal class MessageHeaders
    {
        public string CorellationId { get; set; }
    }
}
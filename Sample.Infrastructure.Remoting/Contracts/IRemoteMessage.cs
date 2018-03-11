namespace Sample.Infrastructure.Remoting.Contracts
{
    public interface IRemoteMessage
    {
        MessageHeaders Headers { get; }
    }

    public class MessageHeaders
    {
        public string RoutingKey { get; set; }
        public string CorrelationId { get; set; }
        public string ReplyTo { get; set; }
    }
}
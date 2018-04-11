namespace Milde.Remoting.Contracts
{
    public class MessageHeaders
    {
        public string RoutingKey { get; set; }
        public string CorrelationId { get; set; }
        public string ReplyTo { get; set; }
    }
}
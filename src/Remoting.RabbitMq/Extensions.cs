using Milde.Remoting.Contracts;
using RabbitMQ.Client;

namespace Milde.Remoting.Rabbit
{
    internal static class Extensions
    {
        public static void SetFrom(this IBasicProperties props, MessageHeaders properties)
        {
            if (properties == null || props == null)
                return;


            if (!string.IsNullOrEmpty(properties.CorrelationId))
                props.CorrelationId = properties.CorrelationId;

            if (!string.IsNullOrEmpty(properties.ReplyTo))
                props.ReplyTo = properties.ReplyTo;
        }
    }
}
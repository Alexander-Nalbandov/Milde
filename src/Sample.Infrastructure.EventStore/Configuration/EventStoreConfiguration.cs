using Microsoft.Extensions.Configuration;

namespace Sample.Infrastructure.EventStore.Configuration
{
    public class EventStoreConfiguration
    {
        public EventStoreConfiguration(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public EventStoreConfiguration(IConfiguration config)
        {
            var host = config["EventStore:Host"];
            var port = int.Parse(config["EventStore:Port"]);

            this.Host = host;
            this.Port = port;
        }

        public string Host { get; private set; }
        public int Port { get; private set; }
    }
}

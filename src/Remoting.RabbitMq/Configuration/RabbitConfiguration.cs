using Microsoft.Extensions.Configuration;

namespace Milde.Remoting.Rabbit.Configuration
{
    internal class RabbitConfiguration
    {
        //TODO: Get the connection settings from the configuration action instead of directly querying IConfiguration
        public RabbitConfiguration(IConfiguration config)
        {
            var rabbitConfig = config.GetSection("rabbit");
            Address = rabbitConfig["address"];
            Username = rabbitConfig["username"];
            Password = rabbitConfig["password"];
            VirtualHost = rabbitConfig["vhost"];
        }

        public string Address { get; }
        public string Username { get; }
        public string Password { get; }
        public string VirtualHost { get; }

        public const string ResponseExchange = "RPC-Response";
        public const string RequestExchange = "RPC-Request";
    }
}
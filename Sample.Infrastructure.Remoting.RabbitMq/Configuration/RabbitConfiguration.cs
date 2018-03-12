using Microsoft.Extensions.Configuration;

namespace Sample.Infrastructure.Remoting.Rabbit.Configuration
{
    internal class RabbitConfiguration
    {
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
    }
}
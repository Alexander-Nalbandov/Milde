using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Sample.Infrastructure.Hosting.Host
{
    internal class Host : IHost
    {
        private readonly IContainer _container;
        private readonly IConfiguration _configuration;

        public Host(IContainer container, IConfiguration configuration)
        {
            _container = container;
            _configuration = configuration;
        }

        public Task Run(Func<IContainer, IConfiguration, Task> action)
        {
            return action(this._container, this._configuration);
        }
    }
}
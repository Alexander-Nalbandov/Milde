using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Sample.Infrastructure.Hosting.Host
{
    public interface IHost
    {
        Task Run(Func<IContainer, IConfiguration, Task> action);
    }
}
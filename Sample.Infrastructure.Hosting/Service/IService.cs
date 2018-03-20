using Autofac;
using Microsoft.Extensions.Configuration;

namespace Sample.Infrastructure.Hosting.Service
{
    public interface IService
    {
        void RegisterDependencies(ContainerBuilder builder, IConfiguration config);
    }
}
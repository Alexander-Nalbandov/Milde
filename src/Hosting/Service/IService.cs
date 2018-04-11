using Autofac;
using Microsoft.Extensions.Configuration;

namespace Milde.Hosting.Service
{
    public interface IService
    {
        void RegisterDependencies(ContainerBuilder builder, IConfiguration config);
    }
}
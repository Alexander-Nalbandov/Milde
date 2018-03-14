using Sample.Infrastructure.Remoting.Communication;

namespace Sample.Infrastructure.Remoting.Client
{
    public class ServiceProxyFactory
    {
        public static TInterface Create<TInterface>(RemoteProcedureExecutor<TInterface> executor, ResponseConverter converter)
        {
            return ServiceProxy<TInterface>.Create(executor, converter);
        }
    }
}
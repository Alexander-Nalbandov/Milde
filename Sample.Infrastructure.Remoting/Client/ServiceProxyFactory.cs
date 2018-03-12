using Sample.Infrastructure.Remoting.Communication;

namespace Sample.Infrastructure.Remoting.Client
{
    public class ServiceProxyFactory
    {
        public static TInterface Create<TInterface>(RemoteProcedureExecutor<TInterface> executor)
        {
            return ServiceProxy<TInterface>.Create(executor);
        }
    }
}
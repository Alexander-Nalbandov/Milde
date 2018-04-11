namespace Milde.Remoting.Client
{
    internal class ServiceProxyFactory
    {
        public static TInterface Create<TInterface>(RemoteProcedureExecutor<TInterface> executor,
            ResponseConverter converter)
        {
            return ServiceProxy<TInterface>.Create(executor, converter);
        }
    }
}
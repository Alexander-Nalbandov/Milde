namespace Sample.Infrastructure.Remoting.Registration
{
    public interface IServiceConfigurator
    {
        void RegisterProxy<TInterface>();

        void RegisterService<TImplementation, TInterface>() where TImplementation : TInterface;
    }
}
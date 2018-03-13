namespace Sample.Infrastructure.Remoting.Registration
{
    public interface IServiceConfigurator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        void RegisterProxy<TInterface>();

        void RegisterService<TImplementation, TInterface>() where TImplementation : TInterface;
    }
}
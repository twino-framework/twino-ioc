namespace Twino.Ioc
{
    public class ServiceInstanceProviderFactory : IServiceInstanceProviderFactory
    {
        public IServiceInstanceProvider Create(ITwinoServiceProvider provider)
        {
            return new ServiceInstanceProvider(provider);
        }
    }
}
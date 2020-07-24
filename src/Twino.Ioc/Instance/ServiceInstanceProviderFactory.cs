namespace Twino.Ioc.Instance
{
    /// <summary>
    /// Creates new Service Instance Provider
    /// </summary>
    public class ServiceInstanceProviderFactory : IServiceInstanceProviderFactory
    {
        /// <summary>
        /// Uses Twino Service Provider and creates new instance provider
        /// </summary>
        public IServiceInstanceProvider Create(ITwinoServiceProvider provider)
        {
            return new ServiceInstanceProvider(provider);
        }
    }
}
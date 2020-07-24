namespace Twino.Ioc.Instance
{
    /// <summary>
    /// Implementation for creating new Service Instance Provider
    /// </summary>
    public interface IServiceInstanceProviderFactory
    {
        /// <summary>
        /// Uses Twino Service Provider and creates new instance provider
        /// </summary>
        IServiceInstanceProvider Create(ITwinoServiceProvider provider);
    }
}
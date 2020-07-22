namespace Twino.Ioc
{
    public interface IServiceInstanceProviderFactory
    {
        IServiceInstanceProvider Create(ITwinoServiceProvider provider);
    }
}
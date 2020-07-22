using System;
using System.Threading.Tasks;

namespace Twino.Ioc
{
    /// <summary>
    /// Twino Service Provider used for checking or getting registered services
    /// </summary>
    public interface ITwinoServiceProvider
    {
        #region Get

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        Task<TService> Get<TService>(IContainerScope scope = null)
            where TService : class;

        /// <summary>
        /// Try gets the service from the container.
        /// </summary>
        Task<bool> TryGet<TService>(out TService service, IContainerScope scope = null)
            where TService : class;

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        Task<object> Get(Type serviceType, IContainerScope scope = null);

        /// <summary>
        /// Try gets the service from the container.
        /// </summary>
        Task<bool> TryGet(Type serviceType, out object service, IContainerScope scope = null);

        #endregion

        #region Descriptor

        /// <summary>
        /// Gets descriptor of type
        /// </summary>
        ServiceDescriptor GetDescriptor<TService>();

        /// <summary>
        /// Gets descriptor of type
        /// </summary>
        ServiceDescriptor GetDescriptor(Type serviceType);

        #endregion

        #region Contains

        /// <summary>
        /// Check service is in container.
        /// </summary>
        bool Contains(Type serviceType);

        /// <summary>
        /// Check service is in container.
        /// </summary>
        bool Contains<T>();

        #endregion
    }
}
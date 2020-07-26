using System;

namespace Twino.Ioc
{
    /// <summary>
    /// Twino Service Provider used for checking or getting registered services
    /// </summary>
    public interface ITwinoServiceProvider : IServiceProvider
    {
        #region Get

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        TService Get<TService>(IContainerScope scope = null)
            where TService : class;

        /// <summary>
        /// Tries to get the service from the container.
        /// </summary>
        bool TryGet<TService>(out TService service, IContainerScope scope = null)
            where TService : class;

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        object Get(Type serviceType, IContainerScope scope = null);

        /// <summary>
        /// Tries to get the service from the container.
        /// </summary>
        bool TryGet(Type serviceType, out object service, IContainerScope scope = null);

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
        
        /// <summary>
        /// Creates new scope
        /// </summary>
        IContainerScope CreateScope();
        
        /// <summary>
        /// Releases item from pool's locked item list
        /// </summary>
        void ReleasePoolItem<TService>(TService service);

    }
}
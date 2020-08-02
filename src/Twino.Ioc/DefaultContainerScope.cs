using System;
using System.Collections.Generic;
using Twino.Ioc.Pool;

namespace Twino.Ioc
{
    /// <summary>
    /// MVC Scope implementation for service container
    /// </summary>
    internal class DefaultContainerScope : IContainerScope
    {
        /// <summary>
        /// All created scoped services in this scope
        /// </summary>
        private readonly Dictionary<Type, object> _scopedServices = new Dictionary<Type, object>();

        /// <summary>
        /// All locked pool instances in this scope
        /// </summary>
        private readonly Dictionary<IServicePool, List<PoolServiceDescriptor>> _poolInstances = new Dictionary<IServicePool, List<PoolServiceDescriptor>>();

        private readonly TwinoServiceProvider _provider;

        public DefaultContainerScope(TwinoServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Puts and instance into the scope
        /// </summary>
        /// <param name="serviceType">Item service type</param>
        /// <param name="instance">Item instance</param>
        public void PutItem(Type serviceType, object instance)
        {
            lock (_scopedServices)
                _scopedServices.Add(serviceType, instance);
        }

        /// <summary>
        /// Gets the service from the container
        /// </summary>
        public TService Get<TService>() where TService : class
        {
            return (TService) Get(typeof(TService));
        }

        /// <summary>
        /// Gets the service from the provider
        /// </summary>
        public object Get(Type serviceType)
        {
            if (_scopedServices.ContainsKey(serviceType))
                return _scopedServices[serviceType];

            object service = _provider.Get(serviceType, true, this);
            _scopedServices.Add(serviceType, service);
            return service;
        }

        /// <summary>
        /// Adds a pool instance to using list
        /// This instance will be released while disposing
        /// </summary>
        public void UsePoolItem(IServicePool pool, PoolServiceDescriptor descriptor)
        {
            lock (_poolInstances)
            {
                if (_poolInstances.ContainsKey(pool))
                {
                    if (pool.Type == ImplementationType.Transient)
                        _poolInstances[pool].Add(descriptor);
                }
                else
                    _poolInstances.Add(pool, new List<PoolServiceDescriptor> {descriptor});
            }
        }

        /// <summary>
        /// Releases all source and using pool instances
        /// </summary>
        public void Dispose()
        {
            lock (_poolInstances)
            {
                foreach (var kv in _poolInstances)
                {
                    foreach (PoolServiceDescriptor descriptor in kv.Value)
                        kv.Key.Release(descriptor);
                }

                _poolInstances.Clear();
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Twino.Ioc.Exceptions;
using Twino.Ioc.Pool;

namespace Twino.Ioc
{
    /// <summary>
    /// Default service container of Twino MVC for dependency inversion
    /// </summary>
    public class ServiceContainer : IServiceContainer, IDisposable
    {
        /// <summary>
        /// Service descriptor items
        /// </summary>
        private readonly List<TwinoServiceDescriptor> _items;

        /// <summary>
        /// Currency service provider
        /// </summary>
        private TwinoServiceProvider _provider;

        /// <summary>
        /// Returns current service provider
        /// </summary>
        /// <returns></returns>
        public ITwinoServiceProvider GetProvider()
        {
            if (_provider == null)
            {
                TwinoServiceProvider provider = new TwinoServiceProvider();
                provider.Build(_items);
                _provider = provider;
            }

            return _provider;
        }

        #region Init - Check - Dispose

        /// <summary>
        /// Creates new service container
        /// </summary>
        public ServiceContainer()
        {
            _items = new List<TwinoServiceDescriptor>();
        }

        /// <summary>
        /// Disposes service container and releases all sources
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region Add Transient

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            AddTransient(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddTransient(typeof(TService), typeof(TImplementation), typeof(TProxy));
        }


        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient<TService, TImplementation>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
        {
            AddTransient(typeof(TService), typeof(TImplementation), null, afterCreated);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient<TService, TImplementation, TProxy>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddTransient(typeof(TService), typeof(TImplementation), typeof(TProxy), afterCreated);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient(Type serviceType, Type implementationType)
        {
            AddTransient(serviceType, implementationType, null, null);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient(Type serviceType, Type implementationType, Type proxyType)
        {
            AddTransient(serviceType, implementationType, proxyType, null);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddTransient(Type serviceType, Type implementationType, Type proxyType, Delegate afterCreated)
        {
            if (_items.Any(x => x.ServiceType == serviceType))
                throw new DuplicateTypeException($"{serviceType.ToTypeString()} service type is already added into service container");

            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Transient, serviceType, implementationType)
                                                {
                                                    ProxyType = proxyType,
                                                    ProxyInstance = null,
                                                    AfterCreatedMethod = afterCreated
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        #endregion

        #region Add Scoped

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            AddScoped(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddScoped(typeof(TService), typeof(TImplementation), typeof(TProxy));
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped<TService, TImplementation>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
        {
            AddScoped(typeof(TService), typeof(TImplementation), null, afterCreated);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped<TService, TImplementation, TProxy>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddScoped(typeof(TService), typeof(TImplementation), typeof(TProxy), afterCreated);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped(Type serviceType, Type implementationType)
        {
            AddScoped(serviceType, implementationType, null, null);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped(Type serviceType, Type implementationType, Type proxyType)
        {
            AddScoped(serviceType, implementationType, proxyType, null);
        }

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        public void AddScoped(Type serviceType, Type implementationType, Type proxyType, Delegate afterCreated)
        {
            if (_items.Any(x => x.ServiceType == serviceType))
                throw new DuplicateTypeException($"{serviceType.ToTypeString()} service type is already added into service container");

            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Scoped, serviceType, implementationType)
                                                {
                                                    ProxyType = proxyType,
                                                    ProxyInstance = null,
                                                    AfterCreatedMethod = afterCreated
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        #endregion

        #region Add Singleton

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            AddSingleton(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddSingleton(typeof(TService), typeof(TImplementation), typeof(TProxy));
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton<TService, TImplementation>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
        {
            AddSingleton(typeof(TService), typeof(TImplementation), afterCreated);
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton<TService, TImplementation, TProxy>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddSingleton(typeof(TService), typeof(TImplementation), typeof(TProxy), afterCreated);
        }

        /// <summary>
        /// Adds a singleton service with instance to the container.
        /// </summary>
        public void AddSingleton<TService, TImplementation>(TImplementation instance)
            where TService : class
            where TImplementation : class, TService
        {
            AddSingleton(typeof(TService), instance);
        }

        /// <summary>
        /// Adds a singleton service with instance to the container.
        /// </summary>
        public void AddSingleton<TService>(TService instance)
            where TService : class
        {
            AddSingleton<TService, TService>(instance);
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton(Type serviceType, Type implementationType)
        {
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton, serviceType, implementationType);
            _items.Add(descriptor);
            _provider = null;
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton(Type serviceType, Type implementationType, Type proxyType)
        {
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton, serviceType, implementationType)
                                                {
                                                    ProxyType = proxyType,
                                                    ProxyInstance = null
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton(Type serviceType, Type implementationType, Delegate afterCreated)
        {
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton, serviceType, implementationType)
                                                {
                                                    AfterCreatedMethod = afterCreated
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        public void AddSingleton(Type serviceType, Type implementationType, Type proxyType, Delegate afterCreated)
        {
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton, serviceType, implementationType)
                                                {
                                                    ProxyType = proxyType,
                                                    ProxyInstance = null,
                                                    AfterCreatedMethod = afterCreated
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        /// <summary>
        /// Adds a singleton service with instance to the container.
        /// </summary>
        public void AddSingleton(Type serviceType, object instance)
        {
            Type implementationType = instance.GetType();
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton, serviceType, implementationType, instance);
            _items.Add(descriptor);
            _provider = null;
        }

        #endregion

        #region Add Pool

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        public void AddTransientPool<TService>() where TService : class
        {
            AddTransientPool<TService, TService>(null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        public void AddTransientPool<TService>(Action<ServicePoolOptions> options)
            where TService : class
        {
            AddPool<TService, TService>(ImplementationType.Transient, options, null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        public void AddTransientPool<TService>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
        {
            AddPool<TService, TService>(ImplementationType.Transient, options, instance);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        public void AddTransientPool<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            AddTransientPool<TService, TImplementation>(null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        public void AddTransientPool<TService, TImplementation>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService
        {
            AddPool<TService, TImplementation>(ImplementationType.Transient, options, null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        public void AddTransientPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddPool<TService, TImplementation, TProxy>(ImplementationType.Transient, options, null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        public void AddTransientPool<TService, TImplementation>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
        {
            AddPool<TService, TImplementation>(ImplementationType.Transient, options, instance);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        public void AddTransientPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddPool<TService, TImplementation, TProxy>(ImplementationType.Transient, options, instance);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        public void AddScopedPool<TService>() where TService : class
        {
            AddScopedPool<TService, TService>(null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        public void AddScopedPool<TService>(Action<ServicePoolOptions> options)
            where TService : class
        {
            AddPool<TService, TService>(ImplementationType.Scoped, options, null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        public void AddScopedPool<TService>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
        {
            AddPool<TService, TService>(ImplementationType.Scoped, options, instance);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        public void AddScopedPool<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            AddScopedPool<TService, TImplementation>(null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        public void AddTransientPool<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddScopedPool<TService, TImplementation, TProxy>(null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        public void AddScopedPool<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddScopedPool<TService, TImplementation, TProxy>(null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        public void AddScopedPool<TService, TImplementation>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService
        {
            AddPool<TService, TImplementation>(ImplementationType.Scoped, options, null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        public void AddScopedPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddPool<TService, TImplementation, TProxy>(ImplementationType.Scoped, options, null);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        public void AddScopedPool<TService, TImplementation>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
        {
            AddPool<TService, TImplementation>(ImplementationType.Scoped, options, instance);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        public void AddScopedPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            AddPool<TService, TImplementation, TProxy>(ImplementationType.Scoped, options, instance);
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="type">Implementation type</param>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        private void AddPool<TService, TImplementation>(ImplementationType type, Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
        {
            ServicePool<TService, TImplementation> pool = new ServicePool<TService, TImplementation>(type, this, options, instance);
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton,
                                                                           typeof(TService), 
                                                                           typeof(TImplementation),
                                                                           //typeof(ServicePool<TService, TImplementation>), 
                                                                           pool)
                                                {
                                                    IsPool = true
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        /// <summary>
        /// Adds a service pool to the container
        /// </summary>
        /// <param name="type">Implementation type</param>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        private void AddPool<TService, TImplementation, TProxy>(ImplementationType type, Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy
        {
            ServicePool<TService, TImplementation, TProxy> pool = new ServicePool<TService, TImplementation, TProxy>(type, this, options, instance);
            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(ImplementationType.Singleton, 
                                                                           typeof(TService), 
                                                                           typeof(TImplementation),
                                                                           //typeof(ServicePool<TService, TImplementation>), 
                                                                           pool)
                                                {
                                                    IsPool = true,
                                                    ProxyType = typeof(TProxy)
                                                };

            _items.Add(descriptor);
            _provider = null;
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes the service from the container
        /// </summary>
        public void Remove<TService>()
            where TService : class
        {
            Remove(typeof(TService));
        }

        /// <summary>
        /// Removes the service from the container
        /// </summary>
        public void Remove(Type type)
        {
            _items.RemoveAll(x => x.ServiceType == type);
        }

        #endregion

        #region Helper

        /// <summary>
        /// Check service is in container.
        /// </summary>
        public bool Contains(Type serviceType)
        {
            return _items.Any(x => x.ServiceType == serviceType);
        }

        /// <summary>
        /// Check service is in container.
        /// </summary>
        public bool Contains<T>()
        {
            return Contains(typeof(T));
        }

        private TwinoServiceDescriptor MapServiceDescriptor(ServiceDescriptor item)
        {
            if (item == null)
                throw new NullReferenceException("Service descriptor is null");

            ImplementationType impl;
            switch (item.Lifetime)
            {
                case ServiceLifetime.Scoped:
                    impl = ImplementationType.Scoped;
                    break;
                case ServiceLifetime.Singleton:
                    impl = ImplementationType.Singleton;
                    break;

                default:
                    impl = ImplementationType.Transient;
                    break;
            }

            Type implementationType = item.ImplementationType;
            if (implementationType == null)
            {
                implementationType = item.ImplementationInstance != null
                                         ? item.ImplementationInstance.GetType()
                                         : item.ServiceType;
            }

            TwinoServiceDescriptor descriptor = new TwinoServiceDescriptor(impl, item.ServiceType, implementationType, item.ImplementationInstance)
                                                {
                                                    MicrosoftServiceDescriptor = item,
                                                    ImplementationFactory = item.ImplementationFactory
                                                };

            return descriptor;
        }

        private ServiceDescriptor MapToExtensionDescriptor(TwinoServiceDescriptor descriptor)
        {
            ServiceLifetime lifetime;
            switch (descriptor.Implementation)
            {
                case ImplementationType.Scoped:
                    lifetime = ServiceLifetime.Scoped;
                    break;

                case ImplementationType.Singleton:
                    lifetime = ServiceLifetime.Singleton;
                    if (descriptor.Instance != null)
                        return new ServiceDescriptor(descriptor.ServiceType, descriptor.Instance);

                    break;

                default:
                    lifetime = ServiceLifetime.Transient;
                    break;
            }

            if (descriptor.ImplementationFactory != null)
                return new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationFactory, lifetime);

            return new ServiceDescriptor(descriptor.ServiceType, descriptor.ImplementationType, lifetime);
        }

        #endregion

        #region Microsoft Extensions Implementation

        /// <summary>
        /// Services count of container
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Container read only status. Always false.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns all service descriptors in container
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            foreach (TwinoServiceDescriptor descriptor in _items)
            {
                if (descriptor.MicrosoftServiceDescriptor != null)
                    yield return descriptor.MicrosoftServiceDescriptor;
                else
                    yield return MapToExtensionDescriptor(descriptor);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds new Microsoft Extension service to service container
        /// </summary>
        public void Add(ServiceDescriptor item)
        {
            TwinoServiceDescriptor descriptor = MapServiceDescriptor(item);
            if (descriptor != null)
                _items.Add(descriptor);
        }

        /// <summary>
        /// Inserts new Microsoft Extension service to service container
        /// </summary>
        public void Insert(int index, ServiceDescriptor item)
        {
            Add(item);
        }

        /// <summary>
        /// Removes all services from container
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Returns true is service is registered 
        /// </summary>
        public bool Contains(ServiceDescriptor item)
        {
            return _items.Any(x => x.MicrosoftServiceDescriptor == item);
        }

        /// <summary>
        /// Removes a service implementation
        /// </summary>
        public bool Remove(ServiceDescriptor item)
        {
            var kv = _items.FirstOrDefault(x => x.MicrosoftServiceDescriptor == item);
            if (kv != null)
            {
                _items.Remove(kv);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method is added to IServiceContainer implementation of Microsoft Extensions.
        /// </summary>
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            foreach (TwinoServiceDescriptor descriptor in _items)
            {
                if (descriptor.MicrosoftServiceDescriptor != null)
                    array[arrayIndex] = descriptor.MicrosoftServiceDescriptor;
                else
                    array[arrayIndex] = MapToExtensionDescriptor(descriptor);

                arrayIndex++;
            }
        }

        /// <summary>
        /// Method is added to IServiceContainer implementation of Microsoft Extensions.
        /// </summary>
        public int IndexOf(ServiceDescriptor item)
        {
            int i = 0;
            foreach (var v in _items)
            {
                if (v.MicrosoftServiceDescriptor == item)
                    return i;

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Method is added to IServiceContainer implementation of Microsoft Extensions.
        /// </summary>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        /// <summary>
        /// Method is added to IServiceContainer implementation of Microsoft Extensions.
        /// throws NotImplementedException
        /// </summary>
        public ServiceDescriptor this[int index]
        {
            get => MapToExtensionDescriptor(_items[index]);
            set => throw new NotSupportedException();
        }

        #endregion

        #region Provider

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        public TService Get<TService>(IContainerScope scope = null) where TService : class
        {
            return GetProvider().Get<TService>(scope);
        }

        /// <summary>
        /// Tries to get the service from the container.
        /// </summary>
        public bool TryGet<TService>(out TService service, IContainerScope scope = null) where TService : class
        {
            return GetProvider().TryGet(out service, scope);
        }

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        public object Get(Type serviceType, IContainerScope scope = null)
        {
            return GetProvider().Get(serviceType, scope);
        }

        /// <summary>
        /// Tries to get the service from the container.
        /// </summary>
        public bool TryGet(Type serviceType, out object service, IContainerScope scope = null)
        {
            return GetProvider().TryGet(serviceType, out service, scope);
        }

        /// <summary>
        /// Gets the service from the container.
        /// </summary>
        public object GetService(Type serviceType)
        {
            return GetProvider().GetService(serviceType);
        }

        /// <summary>
        /// Releases item from pool's locked item list
        /// </summary>
        public void ReleasePoolItem<TService>(TService service)
        {
            GetProvider().ReleasePoolItem(service);
        }

        /// <summary>
        /// Creates new scope
        /// </summary>
        public IContainerScope CreateScope()
        {
            return GetProvider().CreateScope();
        }

        #endregion
    }
}
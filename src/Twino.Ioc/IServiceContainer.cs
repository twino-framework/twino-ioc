﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Twino.Ioc.Pool;

namespace Twino.Ioc
{
    /// <summary>
    /// Service container implementation for Dependency Inversion
    /// </summary>
    public interface IServiceContainer : IServiceCollection, IServiceProvider
    {
        #region Add Transient

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient<TService, TImplementation>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient<TService, TImplementation, TProxy>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient(Type serviceType, Type implementationType);

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient(Type serviceType, Type implementationType, Type decoratorType);

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddTransient(Type serviceType, Type implementationType, Type decoratorType, Delegate afterCreated);

        #endregion

        #region Add Scoped

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped<TService, TImplementation, TProxy>()
           where TService : class
           where TImplementation : class, TService
           where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped<TService, TImplementation>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped<TService, TImplementation, TProxy>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped(Type serviceType, Type implementationType);

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped(Type serviceType, Type implementationType, Type proxyType);

        /// <summary>
        /// Adds a service to the container
        /// </summary>
        void AddScoped(Type serviceType, Type implementationType, Type proxyType, Delegate afterCreated);

        #endregion

        #region Add Singleton

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        void AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        void AddSingleton<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a singleton service with instance to the container.
        /// </summary>
        void AddSingleton<TService, TImplementation>(TImplementation instance)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        void AddSingleton<TService>(TService instance)
            where TService : class;

        /// <summary>
        /// Adds a singleton service to container
        /// </summary>
        void AddSingleton<TService, TImplementation>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a singleton service to container
        /// </summary>
        void AddSingleton<TService, TImplementation, TProxy>(Action<TImplementation> afterCreated)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        void AddSingleton(Type serviceType, Type implementationType);

        /// <summary>
        /// Adds a singleton service to the container.
        /// Service will be created with first call.
        /// </summary>
        void AddSingleton(Type serviceType, Type implementationType, Type proxyType);

        /// <summary>
        /// Adds a singleton service with instance to the container.
        /// </summary>
        void AddSingleton(Type serviceType, object instance);

        #endregion

        #region Add Pool

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        void AddTransientPool<TService>()
            where TService : class;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        void AddScopedPool<TService>()
            where TService : class;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        void AddTransientPool<TService>(Action<ServicePoolOptions> options)
            where TService : class;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        void AddScopedPool<TService>(Action<ServicePoolOptions> options)
            where TService : class;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        void AddTransientPool<TService>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class;


        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        void AddScopedPool<TService>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class;


        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        void AddTransientPool<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        void AddTransientPool<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        void AddScopedPool<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        void AddScopedPool<TService, TImplementation, TProxy>()
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        void AddTransientPool<TService, TImplementation>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        void AddTransientPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        void AddScopedPool<TService, TImplementation>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        void AddScopedPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        void AddTransientPool<TService, TImplementation>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a transient service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        void AddTransientPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        void AddScopedPool<TService, TImplementation>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Adds a scoped service pool to the container
        /// </summary>
        /// <param name="options">Options function</param>
        /// <param name="instance">After each instance is created, to do custom initialization, this method will be called.</param>
        void AddScopedPool<TService, TImplementation, TProxy>(Action<ServicePoolOptions> options, Action<TService> instance)
            where TService : class
            where TImplementation : class, TService
            where TProxy : class, IServiceProxy;

        #endregion

        #region Remove - Release

        /// <summary>
        /// Removes the service from the container
        /// </summary>
        void Remove<TService>()
            where TService : class;

        /// <summary>
        /// Removes the service from the container
        /// </summary>
        void Remove(Type type);

        /// <summary>
        /// Releases item from pool's locked item list
        /// </summary>
        void ReleasePoolItem<TService>(TService service);

        #endregion

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

        /// <summary>
        /// Gets descriptor of type
        /// </summary>
        ServiceDescriptor GetDescriptor<TService>();

        /// <summary>
        /// Gets descriptor of type
        /// </summary>
        ServiceDescriptor GetDescriptor(Type serviceType);

        /// <summary>
        /// Check service is in container.
        /// </summary>
        bool Contains(Type serviceType);

        /// <summary>
        /// Check service is in container.
        /// </summary>
        bool Contains<T>();

        #endregion

        #region Instance - Scope

        /// <summary>
        /// Creates new instance of type
        /// </summary>
        Task<object> CreateInstance(Type type, ConstructorInfo[] usableConstructors, IContainerScope scope = null);

        /// <summary>
        /// Creates new scope belong this container.
        /// If your service container does not support scoping, you can throw NotSupportedException
        /// </summary>
        IContainerScope CreateScope();

        #endregion
        
        /// <summary>
        /// Checks all registered services.
        /// Throws exception if there are missing registrations or circular references
        /// </summary>
        public void CheckServices();
    }
}

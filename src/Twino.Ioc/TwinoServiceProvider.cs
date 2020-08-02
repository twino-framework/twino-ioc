using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Twino.Ioc.Exceptions;
using Twino.Ioc.Pool;

namespace Twino.Ioc
{
    internal class TwinoServiceProvider : ITwinoServiceProvider
    {
        private readonly Dictionary<Type, BuiltServiceDescriptor> _services = new Dictionary<Type, BuiltServiceDescriptor>();

        #region Build

        internal List<BuiltServiceDescriptor> GetBuiltServices()
        {
            return _services.Values.ToList();
        }

        internal void Build(IEnumerable<TwinoServiceDescriptor> services)
        {
            BuildItem(new TwinoServiceDescriptor(ImplementationType.Singleton, typeof(IServiceProvider), GetType(), this), services);
            BuildItem(new TwinoServiceDescriptor(ImplementationType.Singleton, typeof(ITwinoServiceProvider), GetType(), this), services);
            BuildItem(new TwinoServiceDescriptor(ImplementationType.Singleton, typeof(IServiceContainer), GetType(), this), services);

            foreach (TwinoServiceDescriptor descriptor in services)
            {
                //skip microsoft's options open generic registrations
                if (descriptor.ImplementationType.IsGenericType && descriptor.ImplementationType.IsGenericTypeDefinition)
                    continue;

                if (OptionsHelper.IsConfigurationType(descriptor.ServiceType))
                {
                    TwinoServiceDescriptor optionsDescriptor = CreateOptionsItem(descriptor);
                    BuildItem(optionsDescriptor, services);
                }
                else
                    BuildItem(descriptor, services);
            }

            foreach (KeyValuePair<Type, BuiltServiceDescriptor> pair in _services)
                FillParameterDescriptors(pair.Value, pair.Value);
        }

        private TwinoServiceDescriptor CreateOptionsItem(TwinoServiceDescriptor descriptor)
        {
            var configFunc = descriptor.ImplementationFactory;
            Type optionsType = descriptor.ServiceType.GetGenericArguments()[0];

            Type openGeneric = typeof(IOptions<>);
            Type optionsServiceType = openGeneric.MakeGenericType(optionsType);

            TwinoServiceDescriptor optionsDescriptor = new TwinoServiceDescriptor(descriptor.Implementation,
                                                                                  optionsServiceType,
                                                                                  optionsServiceType);

            optionsDescriptor.ImplementationFactory = prov =>
            {
                dynamic configure = configFunc(this);
                dynamic options = Activator.CreateInstance(optionsType);
                object optionsInstance = Options.Create(options);
                configure.Configure(options);
                return optionsInstance;
            };

            return optionsDescriptor;
        }

        private BuiltServiceDescriptor BuildItem(TwinoServiceDescriptor descriptor, IEnumerable<TwinoServiceDescriptor> services)
        {
            ConstructorHelper ctorHelper = new ConstructorHelper(services);
            BuiltServiceDescriptor builtDescriptor = new BuiltServiceDescriptor(descriptor.Implementation,
                                                                                descriptor.ServiceType,
                                                                                descriptor.ImplementationType);


            builtDescriptor.Instance = descriptor.Instance;
            builtDescriptor.ImplementationFactory = descriptor.ImplementationFactory;
            builtDescriptor.IsPool = descriptor.IsPool;
            builtDescriptor.AfterCreatedMethod = descriptor.AfterCreatedMethod;

            //we need to find a ctor to create instance
            if (builtDescriptor.IsPool || builtDescriptor.Instance == null && builtDescriptor.ImplementationFactory == null)
            {
                ConstructorInfo constructorInfo = ctorHelper.FindAvailableConstructor(descriptor.ImplementationType);
                if (constructorInfo == null)
                    throw new IocConstructorException($"{descriptor.ServiceType.ToTypeString()} does not have available constructor");

                builtDescriptor.Build(constructorInfo);
            }

            _services.Add(descriptor.ServiceType, builtDescriptor);
            if (builtDescriptor.IsPool)
            {
                if (builtDescriptor.Instance is IServicePoolInternal pool)
                    pool.SetBuiltDescriptor(builtDescriptor);
            }

            return builtDescriptor;
        }

        private void FillParameterDescriptors(BuiltServiceDescriptor descriptor, BuiltServiceDescriptor root)
        {
            if (descriptor.Parameters == null)
                return;

            if (root.Parameters != null && descriptor.Parameters != null)
            {
                if (descriptor.Parameters.Contains(root.ServiceType))
                    throw new CircularReferenceException($"Circular reference between {root.ServiceType.ToTypeString()} and {descriptor.ServiceType.ToTypeString()}");
            }

            foreach (Type type in descriptor.Parameters)
            {
                if (!_services.ContainsKey(type))
                    throw new MissingReferenceException($"{descriptor.ServiceType.ToTypeString()} requires {type.ToTypeString()} but it's not registered");

                BuiltServiceDescriptor parameterDescriptor = _services[type];
                descriptor.ParameterDescriptors.Add(parameterDescriptor);
                FillParameterDescriptors(parameterDescriptor, root);
            }
        }

        #endregion

        #region Get

        public object GetService(Type serviceType)
        {
            return Get(serviceType);
        }

        public TService Get<TService>(IContainerScope scope = null) where TService : class
        {
            return (TService) Get(typeof(TService), scope);
        }

        public object Get(Type serviceType, IContainerScope scope = null)
        {
            return Get(serviceType, false, scope);
        }

        internal object Get(Type serviceType, bool executedFromScope, IContainerScope scope = null)
        {
            //throw new NullReferenceException($"Could not get service from container: {typeof(TService).ToTypeString()}");
            //throw new KeyNotFoundException($"Service type is not found: {serviceType.ToTypeString()}");

            BuiltServiceDescriptor descriptor = _services[serviceType];
            if (descriptor.IsPool)
                return GetFromPool(descriptor, scope);

            if (descriptor.Implementation == ImplementationType.Scoped && scope == null)
                throw new ScopeException($"{serviceType.ToTypeString()} is registered as scoped service but trying to create instance when scope is null");

            if (descriptor.Instance != null)
                return ApplyAfterGet(descriptor, descriptor.Instance);

            object service;
            if (!executedFromScope && scope != null && descriptor.Implementation == ImplementationType.Scoped)
                service = scope.Get(serviceType);
            else
                service = descriptor.CreateInstance(this, scope);

            return ApplyAfterGet(descriptor, service);
        }

        internal object GetWithDescriptor(BuiltServiceDescriptor descriptor, IContainerScope scope)
        {
            if (descriptor.IsPool)
                return GetFromPool(descriptor, scope);

            if (descriptor.Implementation == ImplementationType.Scoped && scope == null)
                throw new ScopeException($"{descriptor.ServiceType.ToTypeString()} is registered as scoped service but trying to create instance when scope is null");

            object service;
            if (scope != null && descriptor.Implementation == ImplementationType.Scoped)
                service = scope.Get(descriptor.ServiceType);
            else
                service = descriptor.CreateInstance(this, scope);

            return ApplyAfterGet(descriptor, service);
        }

        private object ApplyAfterGet(BuiltServiceDescriptor descriptor, object service)
        {
            if (descriptor.ProxyInstance != null)
                service = descriptor.ProxyInstance.Proxy(service);

            else if (descriptor.ProxyType != null)
            {
                IServiceProxy proxy = (IServiceProxy) Get(descriptor.ProxyType);
                service = proxy.Proxy(service);
            }

            if (descriptor.Implementation == ImplementationType.Singleton)
                descriptor.Instance = service;

            return service;
        }

        private object GetFromPool(BuiltServiceDescriptor descriptor, IContainerScope scope)
        {
            IServicePool pool = (IServicePool) descriptor.Instance;
            PoolServiceDescriptor pdesc = pool.GetAndLock(scope);

            if (pdesc == null)
                throw new NullReferenceException($"{descriptor.ServiceType.ToTypeString()} Service is not registered in the container");

            if (pool.Type == ImplementationType.Scoped && scope == null)
                throw new ScopeException($"{descriptor.ServiceType.ToTypeString()} is registered as Scoped but scope parameter is null for IServiceContainer.Get method");

            if (scope != null)
                scope.UsePoolItem(pool, pdesc);

            return pdesc.GetInstance();
        }

        #endregion

        #region Try - Get

        public bool TryGet<TService>(out TService service, IContainerScope scope = null) where TService : class
        {
            if (!Contains<TService>())
            {
                service = null;
                return false;
            }

            service = Get<TService>(scope);
            return true;
        }

        public bool TryGet(Type serviceType, out object service, IContainerScope scope = null)
        {
            if (!Contains(serviceType))
            {
                service = null;
                return false;
            }

            service = Get(serviceType);
            return true;
        }

        #endregion

        #region Contains

        /// <summary>
        /// Returns true if service type is registered
        /// </summary>
        public bool Contains(Type serviceType)
        {
            return _services.ContainsKey(serviceType);
        }

        /// <summary>
        /// Returns true if service type is registered
        /// </summary>
        public bool Contains<T>()
        {
            return Contains(typeof(T));
        }

        #endregion


        /// <summary>
        /// Releases item from pool's locked item list
        /// </summary>
        public void ReleasePoolItem<TService>(TService service)
        {
            BuiltServiceDescriptor descriptor = _services[typeof(TService)];
            IServicePool pool = (IServicePool) descriptor.Instance;
            pool.ReleaseInstance(service);
        }

        /// <summary>
        /// Creates new scope belong this container.
        /// </summary>
        public IContainerScope CreateScope()
        {
            return new DefaultContainerScope(this);
        }
    }
}
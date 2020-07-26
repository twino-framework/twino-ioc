using System;
using Microsoft.Extensions.DependencyInjection;

namespace Twino.Ioc
{
    /// <summary>
    /// Service definition description for the Dependency Inversion Container
    /// </summary>
    public class TwinoServiceDescriptor
    {
        /// <summary>
        /// Service type.
        /// End-user will ask the implementation type with this type.
        /// Usually this is interface type
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Real object type.
        /// Usually end-user doesn't know this type.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// Implementation method
        /// </summary>
        public ImplementationType Implementation { get; }

        /// <summary>
        /// If the descriptor type is Singleton, this object keeps the singleton object.
        /// </summary>
        public object Instance { get; internal set; }

        /// <summary>
        /// Decorator type.
        /// </summary>
        public Type ProxyType { get; set; }

        /// <summary>
        /// Decorator instance
        /// </summary>
        public IServiceProxy ProxyInstance { get; set; }

        /// <summary>
        /// If not null, called for creating instance of the object
        /// </summary>
        public Func<IServiceProvider, object> ImplementationFactory { get; set; }

        /// <summary>
        /// If true, implementation is pool. Instance is type of IServicePool with generic TService template
        /// </summary>
        public bool IsPool { get; set; }

        /// <summary>
        /// This method is called after instance is created
        /// </summary>
        public Delegate AfterCreatedMethod { get; set; }

        /// <summary>
        /// Reference descriptor for Microsoft Extensions implementation
        /// </summary>
        internal ServiceDescriptor MicrosoftServiceDescriptor { get; set; }

        /// <summary>
        /// Creates new service descriptor object
        /// </summary>
        internal TwinoServiceDescriptor(ImplementationType implementation, Type serviceType, Type implementationType, object instance = null)
        {
            Implementation = implementation;
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Instance = instance;
        }
    }
}
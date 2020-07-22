using System;
using Microsoft.Extensions.DependencyInjection;

namespace Twino.Ioc
{
    /// <summary>
    /// Service container implementation for Dependency Inversion
    /// </summary>
    public interface IServiceContainer : ITwinoServiceCollection,
                                         ITwinoServiceProvider,
                                         IServiceCollection,
                                         IServiceProvider
    {
        /// <summary>
        /// Used for creating NEW instance of a registered type
        /// </summary>
        IServiceInstanceProvider InstanceProvider { get; }

        /// <summary>
        /// Creates new scope belong this container.
        /// If your service container does not support scoping, you can throw NotSupportedException
        /// </summary>
        IContainerScope CreateScope();

        /// <summary>
        /// Checks all registered services.
        /// Throws exception if there are missing registrations or circular references
        /// </summary>
        public void CheckServices();
    }
}
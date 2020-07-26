using System;
using Microsoft.Extensions.Options;

namespace Twino.Ioc
{
    /// <summary>
    /// Options provider, finds and provides Microsoft.Extensions.Options registrations
    /// </summary>
    public class OptionsProvider
    {
        private readonly ITwinoServiceCollection _services;

        /// <summary>
        /// Creates new options provider for service container
        /// </summary>
        public OptionsProvider(ITwinoServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// If the service type is an option type returns true
        /// </summary>
        public bool IsOptionsType(Type serviceType)
        {
            if (!serviceType.IsGenericType)
                return false;

            Type openGenericType = serviceType.GetGenericTypeDefinition();
            return openGenericType == typeof(IOptions<>);
        }

        /// <summary>
        /// Returns true if the service type one of Microsoft.Extensions.Options type
        /// </summary>
        public bool IsConfigurationType(Type serviceType)
        {
            if (serviceType.Namespace != null && serviceType.Namespace.Equals("Microsoft.Extensions.Options"))
                return true;
            
            if (serviceType.IsGenericType)
            {
                //there are multiple open generic types
                Type[] types = serviceType.GetGenericArguments();
                if (types.Length == 0)
                    return true;

                Type openGeneric = serviceType.GetGenericTypeDefinition();

                if (openGeneric == typeof(IConfigureOptions<>))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Finds and returns generic IOptions object.
        /// Generic parameter type is serviceType.GetGenericArguments[0]
        /// </summary>
        public object FindOptions(ITwinoServiceProvider provider, Type serviceType)
        {
            if (provider.Contains(serviceType))
                return provider.Get(serviceType);

            Type[] genericArgs = serviceType.GetGenericArguments();
            if (genericArgs.Length < 1)
                return null;

            Type optionsType = genericArgs[0];
            Type configureType = typeof(IConfigureOptions<>);
            configureType = configureType.MakeGenericType(optionsType);

            dynamic configure = provider.Get(configureType);
            if (configure == null)
                return null;

            dynamic options = Activator.CreateInstance(optionsType);
            dynamic optionsServiceType = Options.Create(options);
            configure.Configure(options);

            _services.AddSingleton(serviceType, optionsServiceType);

            return optionsServiceType;
        }
    }
}
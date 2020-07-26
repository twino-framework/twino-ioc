using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Twino.Ioc.Exceptions;

namespace Twino.Ioc
{
    internal class ConstructorHelper
    {
        private readonly IEnumerable<TwinoServiceDescriptor> _services;
        private readonly OptionsProvider _optionsProvider;

        public ConstructorHelper(IEnumerable<TwinoServiceDescriptor> services, OptionsProvider optionsProvider)
        {
            _services = services;
            _optionsProvider = optionsProvider;
        }

        /// <summary>
        /// Finds all usable constructors of the type
        /// </summary>
        internal ConstructorInfo FindAvailableConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
                throw new IocConstructorException($"{type.ToTypeString()} does not have a public constructor");

            if (constructors.Length == 1)
                return constructors[0];

            foreach (ConstructorInfo c in constructors)
            {
                TwinoIocConstructorAttribute attr = c.GetCustomAttribute<TwinoIocConstructorAttribute>();
                if (attr == null)
                    return c;
            }

            return FindPossibleConstructor(constructors);
        }

        /// <summary>
        /// Finds a possible constructor that can be injected
        /// </summary>
        private ConstructorInfo FindPossibleConstructor(ConstructorInfo[] constructors)
        {
            foreach (ConstructorInfo ctor in constructors)
            {
                bool skip = false;
                ParameterInfo[] parameters = ctor.GetParameters();
                foreach (ParameterInfo parameter in parameters)
                {
                    bool paramFound = _services.Any(x => x.ServiceType == parameter.ParameterType || x.ImplementationType == parameter.ParameterType);
                    if (!paramFound && !_optionsProvider.IsOptionsType(parameter.ParameterType) && !_optionsProvider.IsConfigurationType(parameter.ParameterType))
                    {
                        skip = true;
                        break;
                    }

                    if (parameter.ParameterType.IsPrimitive ||
                        parameter.ParameterType == typeof(decimal) ||
                        parameter.ParameterType == typeof(string))
                    {
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                    return ctor;
            }

            return null;
        }
    }
}
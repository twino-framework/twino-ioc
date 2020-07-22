using System;
using System.Reflection;
using System.Threading.Tasks;
using Twino.Ioc.Exceptions;

namespace Twino.Ioc
{
    public class ServiceInstanceProvider : IServiceInstanceProvider
    {
        private readonly ITwinoServiceProvider _provider;

        public ServiceInstanceProvider(ITwinoServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Creates instance of type.
        /// If it has constructor parameters, finds these parameters from the container
        /// </summary>
        public async Task<object> CreateInstance(Type type, ConstructorInfo[] usableConstructors, IContainerScope scope = null)
        {
            if (usableConstructors == null)
                usableConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            foreach (ConstructorInfo constructor in usableConstructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                //if parameterless create directly and return
                if (parameters.Length == 0)
                    return Activator.CreateInstance(type);

                object[] values = new object[parameters.Length];

                bool failed = false;
                //find all parameters from the container
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo parameter = parameters[i];
                    if (typeof(IContainerScope).IsAssignableFrom(parameter.ParameterType))
                        values[i] = scope;
                    else
                    {
                        try
                        {
                            object value = await _provider.Get(parameter.ParameterType, scope);

                            values[i] = value;
                        }
                        catch (IocConstructorException)
                        {
                            //parameter is not registered in service container
                            //skip to next ctor
                            failed = true;
                            break;
                        }
                    }
                }

                //skip to next ctor
                if (failed)
                    continue;

                //create with parameters found from the container
                return Activator.CreateInstance(type, values);
            }

            throw new IocConstructorException($"{type.ToTypeString()} has no valid constructor");
        }
    }
}
using System;
using Microsoft.Extensions.Options;

namespace Twino.Ioc
{
    internal static class OptionsHelper
    {
        /// <summary>
        /// If the service type is an option type returns true
        /// </summary>
        public static bool IsOptionsType(Type serviceType)
        {
            if (!serviceType.IsGenericType)
                return false;

            Type openGenericType = serviceType.GetGenericTypeDefinition();
            return openGenericType == typeof(IOptions<>);
        }

        /// <summary>
        /// Returns true if the service type one of Microsoft.Extensions.Options type
        /// </summary>
        public static bool IsConfigurationType(Type serviceType)
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
    }
}
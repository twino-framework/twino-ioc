using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Twino.Ioc.Exceptions;

namespace Twino.Ioc
{
    /// <summary>
    /// Helper methods for Twino Ioc
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Creates readable string fullname of a type with generic parameters
        /// </summary>
        internal static string ToTypeString(this Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;

            StringBuilder builder = new StringBuilder();

            builder.Append(type.Namespace);
            builder.Append(".");
            builder.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
            builder.Append("<");

            Type[] generics = type.GetGenericArguments();
            for (int i = 0; i < generics.Length; i++)
            {
                Type g = generics[i];
                builder.Append(g.ToTypeString());
                if (i < generics.Length - 1)
                    builder.Append(",");
            }

            builder.Append(">");

            return builder.ToString();
        }

    }
}
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Twino.Ioc.Instance
{
    /// <summary>
    /// Implementation for creating new instance for registered service
    /// </summary>
    public interface IServiceInstanceProvider
    {
        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="type">Implementation type</param>
        /// <param name="usableConstructors">Usable constructors for container</param>
        /// <param name="scope">Scope value if execution in a scope</param>
        /// <returns>Service instance</returns>
        Task<object> CreateInstance(Type type, ConstructorInfo[] usableConstructors, IContainerScope scope = null);
    }
}
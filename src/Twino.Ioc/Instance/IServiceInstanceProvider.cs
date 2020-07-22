using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Twino.Ioc
{
    public interface IServiceInstanceProvider
    {
        Task<object> CreateInstance(Type type, ConstructorInfo[] usableConstructors, IContainerScope scope = null);
    }
}
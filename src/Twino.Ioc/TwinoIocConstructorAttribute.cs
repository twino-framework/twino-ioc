using System;

namespace Twino.Ioc
{
    /// <summary>
    /// That attribute is for marking constructors of types that are used for creating instance of implementation types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class TwinoIocConstructorAttribute : Attribute
    {
    }
}
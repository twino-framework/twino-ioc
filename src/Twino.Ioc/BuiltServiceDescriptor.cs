using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Twino.Ioc.Pool;

namespace Twino.Ioc
{
    internal class BuiltServiceDescriptor
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
        /// Decorator instance
        /// </summary>
        public IServiceProxy ProxyInstance { get; set; }

        /// <summary>
        /// Decorator type.
        /// </summary>
        public Type ProxyType { get; set; }

        /// <summary>
        /// If the descriptor type is Singleton, this object keeps the singleton object.
        /// </summary>
        public object Instance { get; internal set; }

        /// <summary>
        /// If true, implementation is pool. Instance is type of IServicePool with generic TService template
        /// </summary>
        public bool IsPool { get; set; }

        /// <summary>
        /// If not null, called for creating instance of the object
        /// </summary>
        public Func<IServiceProvider, object> ImplementationFactory { get; set; }

        /// <summary>
        /// This method is called after instance is created
        /// </summary>
        public Delegate AfterCreatedMethod { get; set; }

        public List<BuiltServiceDescriptor> ParameterDescriptors { get; } = new List<BuiltServiceDescriptor>();

        public Type[] Parameters { get; private set; }

        private Func<object> _instanceCreatorFunctionParameterless;
        private Func<object[], object> _instanceCreatorFunction;

        public BuiltServiceDescriptor(ImplementationType implementation, Type serviceType, Type implementationType)
        {
            Implementation = implementation;
            ServiceType = serviceType;
            ImplementationType = implementationType;
        }

        public object CreateInstance(TwinoServiceProvider provider, IContainerScope scope = null)
        {
            if (!IsPool && Instance != null)
                return Instance;

            object service;
            if (ImplementationFactory != null)
            {
                service = ImplementationFactory(provider);
                ExecuteAfterCreated(service);
                return service;
            }

            if (Parameters == null || Parameters.Length == 0)
            {
                service = _instanceCreatorFunctionParameterless();
                ExecuteAfterCreated(service);
                return service;
            }

            object[] p = new object[ParameterDescriptors.Count];
            for (int i = 0; i < p.Length; i++)
            {
                BuiltServiceDescriptor descriptor = ParameterDescriptors[i];
                p[i] = provider.GetWithDescriptor(descriptor, scope);
            }

            service = _instanceCreatorFunction(p);
            ExecuteAfterCreated(service);
            return service;
        }

        public void ExecuteAfterCreated(object service)
        {
            if (AfterCreatedMethod != null)
                AfterCreatedMethod.DynamicInvoke(service);
        }

        internal void Build(ConstructorInfo constructor)
        {
            Parameters = constructor.GetParameters().Select(x => x.ParameterType).ToArray();

            var pExp = Expression.Parameter(typeof(object[]));
            var ctorParams = constructor.GetParameters();

            if (ctorParams.Length == 0)
            {
                var newExpr = Expression.New(constructor);
                _instanceCreatorFunctionParameterless = Expression.Lambda(newExpr).Compile() as Func<object>;
            }
            else
            {
                var argExpressions = new Expression[ctorParams.Length];
                for (var i = 0; i < Parameters.Length; i++)
                {
                    var indexedAcccess = Expression.ArrayIndex(pExp, Expression.Constant(i));
                    argExpressions[i] = Expression.Convert(indexedAcccess, Parameters[i]);
                }

                var newExpr = Expression.New(constructor, argExpressions);
                _instanceCreatorFunction = Expression.Lambda(newExpr, pExp).Compile() as Func<object[], object>;
            }
        }
    }
}
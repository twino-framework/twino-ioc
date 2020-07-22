using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Twino.Ioc.Exceptions;

namespace Twino.Ioc
{
    /// <summary>
    /// Used for checking container registrations
    /// </summary>
    internal class ServiceChecker
    {
        /// <summary>
        /// Options provider is used for Microsoft Extensions integrations
        /// </summary>
        private readonly OptionsProvider _optionsProvider;

        /// <summary>
        /// All service registrations
        /// </summary>
        private readonly IEnumerable<ServiceDescriptor> _descriptors;

        /// <summary>
        /// Root references
        /// </summary>
        private readonly List<ReferenceTree> _tree = new List<ReferenceTree>();

        /// <summary>
        /// Creates new service checker
        /// </summary>
        public ServiceChecker(IEnumerable<ServiceDescriptor> descriptors, OptionsProvider optionsProvider)
        {
            _descriptors = descriptors;
            _optionsProvider = optionsProvider;
        }

        /// <summary>
        /// Checks registrations.
        /// Throws exception if there are missing references of circularity
        /// </summary>
        public void Check()
        {
            foreach (ServiceDescriptor descriptor in _descriptors)
            {
                if (_optionsProvider.IsConfigurationType(descriptor.ServiceType))
                    continue;

                ReferenceTree tree = CreateTree(descriptor, null);
                if (tree != null)
                    _tree.Add(tree);
            }

            foreach (ReferenceTree tree in _tree)
                CheckCircularity(tree);
        }

        /// <summary>
        /// Creates new reference free for the service descriptor
        /// </summary>
        private ReferenceTree CreateTree(ServiceDescriptor descriptor, ReferenceTree parentTree)
        {
            //if there is an instance already created or an implementation factory, we don't care how it will be created
            if (descriptor.ImplementationFactory != null || descriptor.Instance != null)
                return new ReferenceTree(parentTree, descriptor.ServiceType);

            if (descriptor.Constructors == null || descriptor.Constructors.Length == 0)
                throw new IocConstructorException($"{descriptor.ImplementationType.ToTypeString()} has no constructors");

            ReferenceTree tree = new ReferenceTree(parentTree, descriptor.ServiceType);

            foreach (ConstructorInfo constructor in descriptor.Constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                foreach (ParameterInfo parameter in parameters)
                {
                    CheckParentCircularity(tree, parameter.ParameterType, tree);

                    ServiceDescriptor childDescriptor = _descriptors.FirstOrDefault(x => x.ServiceType.IsAssignableFrom(parameter.ParameterType));
                    if (childDescriptor == null)
                        throw new MissingReferenceException($"{descriptor.ImplementationType.ToTypeString()} has an unregistered service type {parameter.ParameterType.ToTypeString()}");

                    tree.Leaves.Add(CreateTree(childDescriptor, tree));
                }
            }

            return tree;
        }

        /// <summary>
        /// Checks circularity
        /// </summary>
        private void CheckCircularity(ReferenceTree item)
        {
            //check self reference
            if (item.Parent != null)
                CheckParentCircularity(item.Parent, item.Type, item);

            //check indirect references
            foreach (ReferenceTree child in item.Leaves)
            {
                CheckParentCircularity(item, child.Type, child);
                CheckCircularity(child);
            }
        }

        /// <summary>
        /// Check circularity for parent objects.
        /// This is recursive method and calls itself till root reference
        /// </summary>
        private void CheckParentCircularity(ReferenceTree current, Type type, ReferenceTree entrypoint)
        {
            if (current.Type == type)
                throw new CircularReferenceException($"Circular reference between {entrypoint.Type.ToTypeString()} and {type.ToTypeString()}");

            if (current.Parent == null)
                return;

            CheckParentCircularity(current.Parent, type, entrypoint);
        }
    }
}
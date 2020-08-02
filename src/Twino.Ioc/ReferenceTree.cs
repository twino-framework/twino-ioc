using System;
using System.Collections.Generic;

namespace Twino.Ioc
{
    /// <summary>
    /// Used for checking missing references and circularity.
    /// Represents each object in root or ctor parameter of another object
    /// </summary>
    internal class ReferenceTree
    {
        /// <summary>
        /// Parent object.
        /// If this object represents a root object, parent is null.
        /// </summary>
        public ReferenceTree Parent { get; set; }

        /// <summary>
        /// Object type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Children of the object.
        /// These children are parameters of constructor method of the type
        /// </summary>
        public List<ReferenceTree> Leaves { get; set; }

        /// <summary>
        /// Creates new reference tree
        /// </summary>
        public ReferenceTree(ReferenceTree parent, Type type) : this(parent, type, new List<ReferenceTree>())
        {
        }

        /// <summary>
        /// Creates new reference tree with children
        /// </summary>
        public ReferenceTree(ReferenceTree parent, Type type, List<ReferenceTree> leaves)
        {
            Parent = parent;
            Type = type;
            Leaves = leaves;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onism.Cldr.Extensions;
using ProtoBuf;

namespace Onism.Cldr
{
    [ProtoContract]
    public class CldrTreeNode
    {
        /// <remarks>
        /// This property is set automatically right after deserialization.
        /// </remarks>
        internal CldrTree Tree { get; set; }

        /// <remarks>
        /// This property is set automatically right after deserialization.
        /// </remarks>
        internal CldrTreeNode Parent { get; set; }

        [ProtoMember(1)]
        public Dictionary<string, CldrTreeNode> Children = new Dictionary<string, CldrTreeNode>();

        [ProtoMember(2)]
        public Dictionary<int, int> LocaleValues = new Dictionary<int, int>();

        public CldrTreeNode()
        {

        }

        internal CldrTreeNode(CldrTree tree, CldrTreeNode parent)
        {
            Tree = tree;
            Parent = parent;
        }

        public string this[CldrLocale index]
        {
            get
            {
                var localeId = Tree.Locales[index];
                var valueId = LocaleValues[localeId];
                var value = Tree.GetValueById(valueId);
                return value;
            }
        }

        public override int GetHashCode()
        {
            return Children.GetHashCode() ^ LocaleValues.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CldrTreeNode;

            if (other == null)
                return false;

            var childrenEqual = Children.IsSameAs(other.Children);
            var localeValuesEqual = LocaleValues.IsSameAs(other.LocaleValues);

            return childrenEqual && localeValuesEqual;
        }

        /// <summary>
        /// Returns a collection of the ancestor nodes of this node.
        /// </summary>
        /// <returns>A collection of the ancestor nodes of this node.</returns>
        public IEnumerable<CldrTreeNode> Ancestors()
        {
            return GetAncestors(false);
        }

        /// <summary>
        /// Returns a collection of tokens that contain this token, and the ancestors of this token.
        /// </summary>
        /// <returns>A collection of tokens that contain this token, and the ancestors of this token.</returns>
        public IEnumerable<CldrTreeNode> AncestorsAndSelf()
        {
            return GetAncestors(true);
        }

        private IEnumerable<CldrTreeNode> GetAncestors(bool self)
        {
            for (var current = self ? this : Parent; current != null; current = current.Parent)
            {
                yield return current;
            }
        }

        /// <summary>
        /// Gets the name this node's parent uses to label it.
        /// </summary>
        public string Key
        {
            get
            {
                return Parent
                    ?.Children
                    .FirstOrDefault(x => ReferenceEquals(x.Value, this))
                    .Key;
            }
        }

        /// <summary>
        /// Gets the path of this node. 
        /// </summary>
        public string Path
        {
            get
            {
                return AncestorsAndSelf()
                    .Select(x => x.Key)
                    .Reverse()
                    .JoinStrings(".");
            }
        }

        /// <summary>
        /// Selects a tree node using a dot-separated expression.
        /// </summary>
        /// <param name="path">The dot-separated expression.</param>
        public CldrTreeNode SelectNode(string path)
        {
            var nodeNames = path.Split('.');
            return SelectNode(nodeNames);
        }

        private CldrTreeNode SelectNode(string[] remainingNodes)
        {
            // this is the sought node
            if (remainingNodes.IsEmpty())
                return this;

            // we need to seek deeper
            var childName = remainingNodes[0];
            CldrTreeNode child;

            // the sought node does not exist
            if (Children.TryGetValue(childName, out child) == false)
                return null;

            // seek deeper
            return child.SelectNode(remainingNodes.Skip(1).ToArray());
        }

        internal void Add(int locale, string[] remainingNodes, int value)
        {
            // this is the sought node
            if (remainingNodes.IsEmpty())
            {
                LocaleValues.Add(locale, value);
                return;
            }

            // we need to seek deeper
            var childName = remainingNodes[0];
            CldrTreeNode child;

            // the sought node does not exist
            if (Children.TryGetValue(childName, out child) == false)
            {
                child = new CldrTreeNode(Tree, this);
                Children.Add(childName, child);
            }

            // seek deeper
            child.Add(locale, remainingNodes.Skip(1).ToArray(), value);
        }

        /*
                public int FindMaxDepth(int currentMax)
        {
            if (Children.IsNotEmpty())
                return Children.Select(x => x.FindMaxDepth(currentMax + 1)).Max();
            return currentMax;
        }
        public IEnumerable<CldrTreeNode> FindVertices()
        {
            var vertices = new List<CldrTreeNode>() { this };
            if (Children.IsNotEmpty())
                vertices.AddRange(Children.SelectMany(x => x.FindVertices()));
            return vertices;
        }
        public IEnumerable<CldrTreeNode> FindValues()
        {
            var leaves = new List<CldrTreeNode>();
            if (Children.IsEmpty())
                leaves.Add(this);
            else
                leaves.AddRange(Children.SelectMany(x => x.FindValues()));
            return leaves;
        }
    */
    }
}
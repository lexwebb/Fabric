using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fabric.Data {
    public class TreeNode<T> where T : class{
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        public TreeNode(T value) {
            Value = value;
        }

        /// <summary>
        /// Gets the <see cref="TreeNode{T}"/> with the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="TreeNode{T}"/>.
        /// </value>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public TreeNode<T> this[int i] => _children[i];

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public TreeNode<T> Parent { get; private set; }

        public T Value { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public ReadOnlyCollection<TreeNode<T>> Children => _children.AsReadOnly();

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public TreeNode<T> AddChild(T value) {
            var node = new TreeNode<T>(value) { Parent = this };
            _children.Add(node);
            return node;
        }

        /// <summary>
        /// Adds the children.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public TreeNode<T>[] AddChildren(params T[] values) {
            return values.Select(AddChild).ToArray();
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public bool RemoveChild(TreeNode<T> node) {
            return _children.Remove(node);
        }

        /// <summary>
        /// Traverses the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Traverse(Action<T> action) {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        /// <summary>
        /// Flattens this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Flatten() {
            return new[] { Value }.Concat(_children.SelectMany(x => x.Flatten()));
        }
    }
}

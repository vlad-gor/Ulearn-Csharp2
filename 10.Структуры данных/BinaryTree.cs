// Вставьте сюда финальное содержимое файла BinaryTree.cs
using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable
    {
        private TreeNode<T> root;

        public T this[int index]
        {
            get
            {
                if (root == null || index < 0 || index >= root.Size) throw new IndexOutOfRangeException();
                var currentNode = root;
                while (true)
                {
                    var leftSize = currentNode.Left?.Size ?? 0;
                    if (index == leftSize)
                        return currentNode.Value;
                    if (index < leftSize)
                        currentNode = currentNode.Left;
                    else
                    {
                        currentNode = currentNode.Right;
                        index -= 1 + leftSize;
                    }
                }
            }  
        }

        public void Add(T key)
        {
            var nodeToAdd = new TreeNode<T>(key);
            if (root == null)
                root = nodeToAdd;
            else
            {
                var currentNode = root;
                while (true)
                {
                    if (currentNode.Value.CompareTo(key) > 0)
                    {
                        if (currentNode.Left == null)
                        {
                            currentNode.Left = new TreeNode<T>(key);
                            break;
                        }
                        currentNode = currentNode.Left;
                    }
                    else
                    {
                        if (currentNode.Right == null)
                        {
                            currentNode.Right = new TreeNode<T>(key);
                            break;
                        }
                        currentNode = currentNode.Right;
                    }
                }
            }
        }

        public bool Contains(T key)
        {
            var currentNode = root;
            while (currentNode != null)
            {
                var diff = currentNode.Value.CompareTo(key);
                if (diff == 0)
                    return true;
                currentNode = diff > 0 ? currentNode.Left : currentNode.Right;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return root?.GetValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class TreeNode<T> where T : IComparable
    {
        public T Value { get; }
        private TreeNode<T> parentNode;
        private TreeNode<T> left;

        public TreeNode<T> Left
        {
            get { return left; }
            set
            {
                if (left != null)
                    OnChildCountChanged(-left.Size);              
                left = value;
                if (value != null)
                {
                    OnChildCountChanged(value.Size);
                    value.parentNode = this;
                }
            }
        }

        private TreeNode<T> right;
        public TreeNode<T> Right
        {
            get { return right; }
            set
            {
                if (right != null)
                    OnChildCountChanged(-right.Size);
                right = value;
                if (value != null)
                {
                    OnChildCountChanged(value.Size);
                    value.parentNode = this;
                }
            }
        }

        public int Size { get; private set; }

        public TreeNode(T value)
        {
            if (value == null)
			    throw new ArgumentNullException(nameof(value));
            Value = value;
            Size = 1;
        }

        public IEnumerable<T> GetValues()
        {
            if (Left != null)
                foreach (var value in Left.GetValues())
                    yield return value;
            yield return Value;
            if (Right != null)
                foreach (var value in Right.GetValues())
                    yield return value;
        }

        private void OnChildCountChanged(int delta)
        {
            Size += delta;
            parentNode?.OnChildCountChanged(delta);
        }
    }
}
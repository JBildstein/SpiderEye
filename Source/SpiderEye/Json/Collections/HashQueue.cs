using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpiderEye.Json.Collections
{
    internal class HashQueue<T>
    {
        public int Count
        {
            get { return store.Count; }
        }

        private readonly HashSet<T> store;
        private Node lastNode;

        public HashQueue()
        {
            store = new HashSet<T>(ReferenceEqualityComparer.Default);
        }

        public bool Push(T value)
        {
            bool unique = store.Add(value);
            lastNode = new Node(value, lastNode);

            return unique;
        }

        public void Pop()
        {
            if (lastNode != null)
            {
                store.Remove(lastNode.Value);
                lastNode = lastNode.Previous;
            }
        }

        public T Peek()
        {
            if (lastNode == null) { return default; }
            else { return lastNode.Value; }
        }

        private sealed class Node
        {
            public T Value { get; }
            public Node Previous { get; }

            public Node(T value, Node previous)
            {
                Value = value;
                Previous = previous;
            }
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<T>
        {
            public static readonly ReferenceEqualityComparer Default = new ReferenceEqualityComparer();

            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}

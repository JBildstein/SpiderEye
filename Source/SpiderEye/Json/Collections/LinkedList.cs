using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpiderEye.Json.Collections
{
    internal sealed partial class LinkedList<T> : IEnumerable<T>, IJsonArray
    {
        public int Count
        {
            get;
            private set;
        }

        private Node first;
        private Node last;

        public void Add(T item)
        {
            if (first == null) { last = first = new Node(item); }
            else { last = last.Next = new Node(item); }

            Count++;
        }

        public void Add(object value)
        {
            Add((T)value);
        }

        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator()
        {
            return new LinkedListEnumerator(first);
        }

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpiderEye.Tools.Json
{
    internal sealed partial class LinkedList<T>
    {
        [DebuggerStepThrough]
        private struct LinkedListEnumerator : IEnumerator<T>
        {
            public T Current
            {
                get
                {
                    if (currentNode != null) { return currentNode.Value; }
                    else { return default; }
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private Node first;
            private Node currentNode;
            private bool hasFinished;

            public LinkedListEnumerator(Node first)
            {
                this.first = first;
                currentNode = null;
                hasFinished = false;
            }

            public bool MoveNext()
            {
                if (hasFinished) { return false; }

                if (currentNode == null) { currentNode = first; }
                else { currentNode = currentNode.Next; }

                hasFinished = currentNode == null;
                return !hasFinished;
            }

            public void Reset()
            {
                hasFinished = false;
                currentNode = null;
            }

            public void Dispose()
            {
                first = null;
                currentNode = null;
            }
        }
    }
}

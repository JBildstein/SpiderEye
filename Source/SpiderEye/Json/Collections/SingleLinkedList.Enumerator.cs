using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpiderEye.Json.Collections
{
    internal sealed partial class SingleLinkedList<T>
    {
        [DebuggerStepThrough]
        private struct SingleLinkedListEnumerator : IEnumerator<T>
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

            public SingleLinkedListEnumerator(Node first)
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

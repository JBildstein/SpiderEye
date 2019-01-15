namespace SpiderEye.Json.Collections
{
    internal sealed partial class LinkedList<T>
    {
        private sealed class Node
        {
            public T Value { get; }
            public Node Next { get; set; }

            public Node(T value)
            {
                Value = value;
            }
        }
    }
}

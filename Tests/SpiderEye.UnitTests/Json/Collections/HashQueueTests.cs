using System;
using Xunit;

namespace SpiderEye.Json.Collections
{
    public class HashQueueTests
    {
        [Fact]
        public void Push_WithDuplicateValue_ReturnsFalse()
        {
            var queue = new HashQueue<object>();
            object value = new object();
            queue.Push(value);

            bool result = queue.Push(value);

            Assert.False(result);
        }

        [Fact]
        public void Push_WithDuplicateValueTypeValue_ReturnsTrue()
        {
            var queue = new HashQueue<object>();
            var value = new DateTime(1990, 11, 26, 1, 2, 3);
            queue.Push(value);

            bool result = queue.Push(value);

            Assert.True(result);
        }

        [Fact]
        public void Push_WithUniqueValue_ReturnsTrue()
        {
            var queue = new HashQueue<object>();
            queue.Push(new object());

            bool result = queue.Push(new object());

            Assert.True(result);
        }

        [Fact]
        public void Count_WithAddedvalues_ReturnsCorrectValue()
        {
            var queue = new HashQueue<int>();
            queue.Push(14);
            queue.Push(28);
            queue.Push(36);

            Assert.Equal(3, queue.Count);
        }

        [Fact]
        public void Peek_WithFilledQueue_ReturnsLastElement()
        {
            var queue = new HashQueue<int>();
            queue.Push(14);
            queue.Push(28);

            int result = queue.Peek();

            Assert.Equal(28, result);
        }

        [Fact]
        public void Pop_WithFilledQueue_RemovesLastElement()
        {
            var queue = new HashQueue<int>();
            queue.Push(14);
            queue.Push(28);
            queue.Push(36);

            queue.Pop();

            Assert.Equal(28, queue.Peek());
        }
    }
}

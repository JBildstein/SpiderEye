using Xunit;

namespace SpiderEye.Json.Collections
{
    public class SingleLinkedListTests
    {
        [Fact]
        public void SingleLinkedList_WithAddedValues_EnumeratesCorrectly()
        {
            var list = new SingleLinkedList<int> { 1, 2, 3 };

            Assert.Equal(new int[] { 1, 2, 3 }, list);
        }

        [Fact]
        public void Count_WithAddedvalues_ReturnsCorrectValue()
        {
            var list = new SingleLinkedList<int>() { 1, 2, 3 };

            Assert.Equal(3, list.Count);
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace SpiderEye.Json.TestModels
{
    internal class ArrayModel
    {
        public static readonly ArrayModel Default = new ArrayModel
        {
            IntArray = new int[] { 1, 2, 3 },
            IntIEnumerable = Enumerable.Range(1, 3),
            IntICollection = new List<int> { 1, 2, 3 },
            IntIList = new List<int> { 1, 2, 3 },
            IntList = new List<int> { 1, 2, 3 },
        };

        public static readonly ArrayModel Null = new ArrayModel
        {
            IntArray = null,
            IntIEnumerable = null,
            IntICollection = null,
            IntIList = null,
            IntList = null,
        };

        public const string DefaultJson = "{" +
            "\"intArray\":[1,2,3]," +
            "\"intIEnumerable\":[1,2,3]," +
            "\"intICollection\":[1,2,3]," +
            "\"intIList\":[1,2,3]," +
            "\"intList\":[1,2,3]" +
            "}";

        public const string NullJson = "{" +
            "\"intArray\":null," +
            "\"intIEnumerable\":null," +
            "\"intICollection\":null," +
            "\"intIList\":null," +
            "\"intList\":null" +
            "}";

        public int[] IntArray { get; set; }
        public IEnumerable<int> IntIEnumerable { get; set; }
        public ICollection<int> IntICollection { get; set; }
        public IList<int> IntIList { get; set; }
        public List<int> IntList { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ArrayModel model)
            {
                return ValuesEqual(IntArray, model.IntArray) &&
                    ValuesEqual(IntIEnumerable, model.IntIEnumerable) &&
                    ValuesEqual(IntICollection, model.IntICollection) &&
                    ValuesEqual(IntIList, model.IntIList) &&
                    ValuesEqual(IntList, model.IntList);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool ValuesEqual(IEnumerable<int> a, IEnumerable<int> b)
        {
            if (a == b) { return true; }
            else if (a == null || b == null) { return false; }

            return a.SequenceEqual(b);
        }
    }
}

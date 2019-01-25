using System.Collections.Generic;
using System.Linq;

namespace SpiderEye.Json.TestModels
{
    internal class NestedModel
    {
        public static readonly NestedModel Default = new NestedModel
        {
            Arrays = ArrayModel.Default,
            ObjectArray = new ObjectModel[]
            {
                new ObjectModel
                {
                    Value = 1,
                    ArrayValue = new int[] { 11, 12, 13 },
                    StructValue = new ValueTypeModel(1000),
                    ObjectValue = new ObjectModel
                    {
                        Value = 100,
                        ArrayValue = new int[] { 101, 102, 103 },
                        StructValue = new ValueTypeModel(2000),
                        ObjectValue = null,
                    },
                },
                new ObjectModel
                {
                    Value = 2,
                    ArrayValue = new int[] { 21, 22, 23 },
                    StructValue = new ValueTypeModel(3000),
                    ObjectValue = null,
                },
            },
        };

        public const string DefaultJson = "{" +
            "\"arrays\":" + ArrayModel.DefaultJson + "," +
            "\"objectArray\":[" +
                "{" +
                    "\"value\":1," +
                    "\"arrayValue\":[11,12,13]," +
                    "\"structValue\":{\"value\":1000}," +
                    "\"objectValue\":" +
                    "{" +
                        "\"value\":100," +
                        "\"arrayValue\":[101,102,103]," +
                        "\"structValue\":{\"value\":2000}," +
                        "\"objectValue\":null" +
                    "}" +
                "}," +
                "{" +
                    "\"value\":2," +
                    "\"arrayValue\":[21,22,23]," +
                    "\"structValue\":{\"value\":3000}," +
                    "\"objectValue\":null" +
                "}" +
            "]}";

        public ArrayModel Arrays { get; set; }
        public ObjectModel[] ObjectArray { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is NestedModel model)
            {
                return ObjectValuesEqual(Arrays, model.Arrays)
                    && ArrayValuesEqual(ObjectArray, model.ObjectArray);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool ObjectValuesEqual<T>(T a, T b)
            where T : class
        {
            if (a == b) { return true; }
            else if (a == null || b == null) { return false; }

            return a.Equals(b);
        }

        private bool ArrayValuesEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a == b) { return true; }
            else if (a == null || b == null) { return false; }

            return a.SequenceEqual(b);
        }
    }
}

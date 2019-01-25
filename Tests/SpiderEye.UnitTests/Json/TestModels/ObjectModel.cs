using System.Collections.Generic;
using System.Linq;

namespace SpiderEye.Json.TestModels
{
    internal class ObjectModel
    {
        public static readonly ObjectModel Recursive = CreateRecursiveModel();

        public int Value { get; set; }
        public int[] ArrayValue { get; set; }
        public ValueTypeModel StructValue { get; set; }
        public ObjectModel ObjectValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ObjectModel model)
            {
                return Value == model.Value
                    && ArrayValuesEqual(ArrayValue, model.ArrayValue)
                    && StructValue == model.StructValue
                    && ObjectValuesEqual(ObjectValue, model.ObjectValue);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private static ObjectModel CreateRecursiveModel()
        {
            var model = new ObjectModel { ObjectValue = new ObjectModel() };
            model.ObjectValue.ObjectValue = model;

            return model;
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

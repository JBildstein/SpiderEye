namespace SpiderEye.Json.TestModels
{
    internal struct ValueTypeModel
    {
        public static readonly ValueTypeModel Default = new ValueTypeModel(123);

        public const string DefaultJson = "{\"value\":123}";

        public readonly int Value;

        public ValueTypeModel(int value)
        {
            Value = value;
        }

        public static bool operator ==(ValueTypeModel a, ValueTypeModel b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(ValueTypeModel a, ValueTypeModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is ValueTypeModel model && this == model;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}

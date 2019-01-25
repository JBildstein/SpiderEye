using System;
namespace SpiderEye.Json.TestModels
{
    internal class ValuesModel
    {
        public static readonly ValuesModel Default = new ValuesModel
        {
            UInt8Property = 12,
            Int8Property = 13,
            Int16Property = 14,
            UInt16Property = 15,
            Int32Property = 16,
            UInt32Property = 17,
            Int64Property = 18,
            UInt64Property = 19,
            FloatProperty = 20.4f,
            DoubleProperty = 21.5d,
            DecimalProperty = 22.6m,
            BoolProperty = true,
            StringProperty = "TestString 2",
            DateTimeProperty = new DateTime(1991, 12, 27, 5, 6, 7, 8, DateTimeKind.Utc),

            UInt8Field = 1,
            Int8Field = 2,
            Int16Field = 3,
            UInt16Field = 4,
            Int32Field = 5,
            UInt32Field = 6,
            Int64Field = 7,
            UInt64Field = 8,
            FloatField = 9.1f,
            DoubleField = 10.2d,
            DecimalField = 11.3m,
            BoolField = true,
            StringField = "TestString 1",
            DateTimeField = new DateTime(1990, 11, 26, 1, 2, 3, 4, DateTimeKind.Utc),
        };

        public const string DefaultJson = "{" +
            "\"uInt8Property\":12," +
            "\"int8Property\":13," +
            "\"int16Property\":14," +
            "\"uInt16Property\":15," +
            "\"int32Property\":16," +
            "\"uInt32Property\":17," +
            "\"int64Property\":18," +
            "\"uInt64Property\":19," +
            "\"floatProperty\":20.4," +
            "\"doubleProperty\":21.5," +
            "\"decimalProperty\":22.6," +
            "\"boolProperty\":true," +
            "\"stringProperty\":\"TestString 2\"," +
            "\"dateTimeProperty\":\"1991-12-27T05:06:07.0080000Z\"," +
            "\"uInt8Field\":1," +
            "\"int8Field\":2," +
            "\"int16Field\":3," +
            "\"uInt16Field\":4," +
            "\"int32Field\":5," +
            "\"uInt32Field\":6," +
            "\"int64Field\":7," +
            "\"uInt64Field\":8," +
            "\"floatField\":9.1," +
            "\"doubleField\":10.2," +
            "\"decimalField\":11.3," +
            "\"boolField\":true," +
            "\"stringField\":\"TestString 1\"," +
            "\"dateTimeField\":\"1990-11-26T01:02:03.0040000Z\"" +
            "}";

        public byte UInt8Property { get; set; }
        public sbyte Int8Property { get; set; }
        public short Int16Property { get; set; }
        public ushort UInt16Property { get; set; }
        public int Int32Property { get; set; }
        public uint UInt32Property { get; set; }
        public long Int64Property { get; set; }
        public ulong UInt64Property { get; set; }
        public float FloatProperty { get; set; }
        public double DoubleProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }

        public byte UInt8Field;
        public sbyte Int8Field;
        public short Int16Field;
        public ushort UInt16Field;
        public int Int32Field;
        public uint UInt32Field;
        public long Int64Field;
        public ulong UInt64Field;
        public float FloatField;
        public double DoubleField;
        public decimal DecimalField;
        public bool BoolField;
        public string StringField;
        public DateTime DateTimeField;

        public override bool Equals(object obj)
        {
            if (obj is ValuesModel model)
            {
                return UInt8Property == model.UInt8Property
                    && Int8Property == model.Int8Property
                    && Int16Property == model.Int16Property
                    && UInt16Property == model.UInt16Property
                    && Int32Property == model.Int32Property
                    && UInt32Property == model.UInt32Property
                    && Int64Property == model.Int64Property
                    && UInt64Property == model.UInt64Property
                    && FloatProperty == model.FloatProperty
                    && DoubleProperty == model.DoubleProperty
                    && DecimalProperty == model.DecimalProperty
                    && BoolProperty == model.BoolProperty
                    && StringProperty == model.StringProperty
                    && DateTimeProperty == model.DateTimeProperty
                    && UInt8Field == model.UInt8Field
                    && Int8Field == model.Int8Field
                    && Int16Field == model.Int16Field
                    && UInt16Field == model.UInt16Field
                    && Int32Field == model.Int32Field
                    && UInt32Field == model.UInt32Field
                    && Int64Field == model.Int64Field
                    && UInt64Field == model.UInt64Field
                    && FloatField == model.FloatField
                    && DoubleField == model.DoubleField
                    && DecimalField == model.DecimalField
                    && BoolField == model.BoolField
                    && StringField == model.StringField
                    && DateTimeField == model.DateTimeField;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

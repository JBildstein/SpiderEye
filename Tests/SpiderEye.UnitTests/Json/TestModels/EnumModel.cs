using System;

namespace SpiderEye.Json.TestModels
{
    internal class EnumModel
    {
        public static readonly EnumModel Default = new EnumModel
        {
            StringProperty = SomeEnum.Prop1,
            NumericProperty = SomeEnum.Prop1,
            FlagsStringProperty = SomeEnum.Prop1 | SomeEnum.Prop2,
            FlagsNumericProperty = SomeEnum.Prop1 | SomeEnum.Prop2,
            NullableStringProperty = SomeEnum.Prop2,
            NullableNumericProperty = SomeEnum.Prop2,
            NullableNullProperty = null,
            StringField = SomeEnum.Field1,
            NumericField = SomeEnum.Field1,
            FlagsStringField = SomeEnum.Field1 | SomeEnum.Field2,
            FlagsNumericField = SomeEnum.Field1 | SomeEnum.Field2,
            NullableStringField = SomeEnum.Field2,
            NullableNumericField = SomeEnum.Field2,
            NullableNullField = null,
        };

        public const string DefaultReadJson = "{" +
            "\"stringProperty\":\"Prop1\"," +
            "\"numericProperty\":1," +
            "\"flagsStringProperty\":\"Prop1, Prop2\"," +
            "\"flagsNumericProperty\":3," +
            "\"nullableStringProperty\":\"Prop2\"," +
            "\"nullableNumericProperty\":2," +
            "\"nullableNullProperty\":null," +
            "\"stringField\":\"Field1\"," +
            "\"numericField\":4," +
            "\"flagsStringField\":\"Field1, Field2\"," +
            "\"flagsNumericField\":12," +
            "\"nullableStringField\":\"Field2\"," +
            "\"nullableNumericField\":8," +
            "\"nullableNullField\":null" +
            "}";

        public const string DefaultWriteJson = "{" +
            "\"stringProperty\":\"Prop1\"," +
            "\"numericProperty\":\"Prop1\"," +
            "\"flagsStringProperty\":\"Prop1, Prop2\"," +
            "\"flagsNumericProperty\":\"Prop1, Prop2\"," +
            "\"nullableStringProperty\":\"Prop2\"," +
            "\"nullableNumericProperty\":\"Prop2\"," +
            "\"nullableNullProperty\":null," +
            "\"stringField\":\"Field1\"," +
            "\"numericField\":\"Field1\"," +
            "\"flagsStringField\":\"Field1, Field2\"," +
            "\"flagsNumericField\":\"Field1, Field2\"," +
            "\"nullableStringField\":\"Field2\"," +
            "\"nullableNumericField\":\"Field2\"," +
            "\"nullableNullField\":null" +
            "}";

        public SomeEnum StringProperty { get; set; }
        public SomeEnum NumericProperty { get; set; }
        public SomeEnum FlagsStringProperty { get; set; }
        public SomeEnum FlagsNumericProperty { get; set; }
        public SomeEnum? NullableStringProperty { get; set; }
        public SomeEnum? NullableNumericProperty { get; set; }
        public SomeEnum? NullableNullProperty { get; set; }

        public SomeEnum StringField;
        public SomeEnum NumericField;
        public SomeEnum FlagsStringField;
        public SomeEnum FlagsNumericField;
        public SomeEnum? NullableStringField;
        public SomeEnum? NullableNumericField;
        public SomeEnum? NullableNullField;

        public override bool Equals(object obj)
        {
            if (obj is EnumModel model)
            {
                return StringProperty == model.StringProperty
                    && NumericProperty == model.NumericProperty
                    && FlagsStringProperty == model.FlagsStringProperty
                    && FlagsNumericProperty == model.FlagsNumericProperty
                    && NullableStringProperty == model.NullableStringProperty
                    && NullableNumericProperty == model.NullableNumericProperty
                    && NullableNullProperty == model.NullableNullProperty
                    && StringField == model.StringField
                    && NumericField == model.NumericField
                    && FlagsStringField == model.FlagsStringField
                    && FlagsNumericField == model.FlagsNumericField
                    && NullableStringField == model.NullableStringField
                    && NullableNumericField == model.NullableNumericField
                    && NullableNullField == model.NullableNullField;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [Flags]
        public enum SomeEnum
        {
            Prop1 = 1 << 0,
            Prop2 = 1 << 1,
            Field1 = 1 << 2,
            Field2 = 1 << 3,
        }
    }
}

using System;
using System.Collections.Generic;
using SpiderEye.Json.TestModels;
using Xunit;
namespace SpiderEye.Json
{
    public partial class JsonConverterTests
    {
        [Fact]
        public void Constructor_WithCacheNull_ThrowsException()
        {
            var exception = Record.Exception(() => new JsonConverter(null));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Serialize_WithRecursiveValue_ThrowsException()
        {
            var converter = new JsonConverter();

            var exception = Record.Exception(() => converter.Serialize(ObjectModel.Recursive));

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Theory]
        [MemberData(nameof(SerializeData))]
        public void Serialize_WithSupportedValues_ReturnsJson(object value, string expected)
        {
            var converter = new JsonConverter();

            string result = converter.Serialize(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(DeserializeData))]
        public void Deserialize_WithSupportedValues_ReturnsJson(object expected, string value, Type type)
        {
            var converter = new JsonConverter();

            object result = converter.Deserialize(value, type);

            Assert.Equal(expected, result);
            if (result != null) { Assert.Equal(result.GetType(), type); }
        }

        public static readonly object[][] SerializeData =
        {
            new object[] { null, "null" },
            new object[] { new int[0], "[]" },
            new object[] { new EmptyObjectModel(), "{}" },
            new object[] { (byte)1, "1" },
            new object[] { (sbyte)2, "2" },
            new object[] { (short)3, "3" },
            new object[] { (ushort)4, "4" },
            new object[] { 5, "5" },
            new object[] { (uint)6, "6" },
            new object[] { (long)7, "7" },
            new object[] { (ulong)8, "8" },
            new object[] { 9.1f, "9.1" },
            new object[] { 10.2d, "10.2" },
            new object[] { 11.3m, "11.3" },
            new object[] { 11.0m, "11.0" },
            new object[] { true, "true" },
            new object[] { false, "false" },
            new object[] { "TestString 1", "\"TestString 1\"" },
            new object[] { "EscapeString: \" \\ \t", "\"EscapeString: \\\" \\\\ \\u0009\"" },
            new object[] { new DateTime(1990, 11, 26, 1, 2, 3, 4, DateTimeKind.Utc), "\"1990-11-26T01:02:03.0040000Z\"" },

            // Note: the following test cases are a bit brittle because the order
            // of the fields and properties is not guaranteed by the .Net Reflection API
            new object[] { ArrayModel.Default, ArrayModel.DefaultJson },
            new object[] { ArrayModel.Null, ArrayModel.NullJson },
            new object[] { NullableValuesModel.Default, NullableValuesModel.DefaultJson },
            new object[] { NullableValuesModel.Null, NullableValuesModel.NullJson },
            new object[] { ValuesModel.Default, ValuesModel.DefaultJson },
            new object[] { NestedModel.Default, NestedModel.DefaultJson },
            new object[] { ValueTypeModel.Default, ValueTypeModel.DefaultJson },
            new object[] { AttributesModel.DefaultWrite, AttributesModel.DefaultWriteJson },
        };

        public static readonly object[][] DeserializeData =
        {
            new object[] { null, "null", typeof(EmptyObjectModel) },
            new object[] { new int[0], "[]", typeof(int[]) },
            new object[] { new List<int>(), "[]", typeof(List<int>) },
            new object[] { new EmptyObjectModel(), "{}", typeof(EmptyObjectModel) },
            new object[] { (byte)1, "1", typeof(byte) },
            new object[] { (sbyte)2, "2", typeof(sbyte) },
            new object[] { (short)3, "3", typeof(short) },
            new object[] { (ushort)4, "4", typeof(ushort) },
            new object[] { 5, "5", typeof(int) },
            new object[] { (uint)6, "6", typeof(uint) },
            new object[] { (long)7, "7", typeof(long) },
            new object[] { (ulong)8, "8", typeof(ulong) },
            new object[] { 9.1f, "9.1", typeof(float) },
            new object[] { 10.2d, "10.2", typeof(double) },
            new object[] { 11.3m, "11.3", typeof(decimal) },
            new object[] { true, "true", typeof(bool) },
            new object[] { false, "false", typeof(bool) },
            new object[] { "TestString 1", "\"TestString 1\"", typeof(string) },
            new object[] { "EscapeString: \" \\ \t", "\"EscapeString: \\\" \\\\ \\u0009\"", typeof(string) },
            new object[] { new DateTime(1990, 11, 26, 1, 2, 3, 4, DateTimeKind.Utc), "\"1990-11-26T01:02:03.0040000Z\"", typeof(DateTime) },
            new object[] { new DateTime(1990, 11, 26, 1, 2, 3, 4, DateTimeKind.Utc), "659581323004", typeof(DateTime) },
            new object[] { new DateTime(1990, 11, 26, 1, 2, 3, 4, DateTimeKind.Utc), "659581323.004", typeof(DateTime) },
            new object[] { ArrayModel.Default, ArrayModel.DefaultJson, typeof(ArrayModel) },
            new object[] { ArrayModel.Null, ArrayModel.NullJson, typeof(ArrayModel) },
            new object[] { NullableValuesModel.Default, NullableValuesModel.DefaultJson, typeof(NullableValuesModel) },
            new object[] { NullableValuesModel.Null, NullableValuesModel.NullJson, typeof(NullableValuesModel) },
            new object[] { ValuesModel.Default, ValuesModel.DefaultJson, typeof(ValuesModel) },
            new object[] { NestedModel.Default, NestedModel.DefaultJson, typeof(NestedModel) },
            new object[] { ValueTypeModel.Default, ValueTypeModel.DefaultJson, typeof(ValueTypeModel) },
            new object[] { AttributesModel.DefaultRead, AttributesModel.DefaultReadJson, typeof(AttributesModel) },
        };
    }
}

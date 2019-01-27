using System;
using System.Collections.Generic;
using SpiderEye.Json.Collections;
using Xunit;
namespace SpiderEye.Json.Helper
{
    public class JsonToolsTests
    {
        [Theory]
        [InlineData(typeof(byte), JsonValueType.Int)]
        [InlineData(typeof(sbyte), JsonValueType.Int)]
        [InlineData(typeof(short), JsonValueType.Int)]
        [InlineData(typeof(ushort), JsonValueType.Int)]
        [InlineData(typeof(int), JsonValueType.Int)]
        [InlineData(typeof(uint), JsonValueType.Int)]
        [InlineData(typeof(long), JsonValueType.Int)]
        [InlineData(typeof(ulong), JsonValueType.Int)]
        [InlineData(typeof(float), JsonValueType.Float)]
        [InlineData(typeof(double), JsonValueType.Float)]
        [InlineData(typeof(decimal), JsonValueType.Float)]
        [InlineData(typeof(bool), JsonValueType.Bool)]
        [InlineData(typeof(string), JsonValueType.String)]
        [InlineData(typeof(DateTime), JsonValueType.DateTime)]
        [InlineData(typeof(DateTimeKind), JsonValueType.Enum)]
        [InlineData(typeof(byte?), JsonValueType.Int)]
        [InlineData(typeof(sbyte?), JsonValueType.Int)]
        [InlineData(typeof(short?), JsonValueType.Int)]
        [InlineData(typeof(ushort?), JsonValueType.Int)]
        [InlineData(typeof(int?), JsonValueType.Int)]
        [InlineData(typeof(uint?), JsonValueType.Int)]
        [InlineData(typeof(long?), JsonValueType.Int)]
        [InlineData(typeof(ulong?), JsonValueType.Int)]
        [InlineData(typeof(float?), JsonValueType.Float)]
        [InlineData(typeof(double?), JsonValueType.Float)]
        [InlineData(typeof(decimal?), JsonValueType.Float)]
        [InlineData(typeof(bool?), JsonValueType.Bool)]
        [InlineData(typeof(DateTime?), JsonValueType.DateTime)]
        [InlineData(typeof(DateTimeKind?), JsonValueType.Enum)]
        [InlineData(typeof(int[]), JsonValueType.Array)]
        [InlineData(typeof(IEnumerable<int>), JsonValueType.Array)]
        [InlineData(typeof(IList<int>), JsonValueType.Array)]
        [InlineData(typeof(ICollection<int>), JsonValueType.Array)]
        [InlineData(typeof(List<int>), JsonValueType.Array)]
        [InlineData(typeof(object), JsonValueType.Object)]
        [InlineData(typeof(JsonToolsTests), JsonValueType.Object)]
        internal void GetJsonType_WithType_ReturnsCorrectJsonType(Type value, JsonValueType expected)
        {
            var result = JsonTools.GetJsonType(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(typeof(int[]), typeof(int))]
        [InlineData(typeof(IEnumerable<int>), typeof(int))]
        [InlineData(typeof(IList<int>), typeof(int))]
        [InlineData(typeof(ICollection<int>), typeof(int))]
        [InlineData(typeof(List<int>), typeof(int))]
        internal void GetArrayValueType_WithType_ReturnsCorrectType(Type value, Type expected)
        {
            var result = JsonTools.GetArrayValueType(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(JsonValueType.Int, true)]
        [InlineData(JsonValueType.Float, true)]
        [InlineData(JsonValueType.Bool, true)]
        [InlineData(JsonValueType.Null, true)]
        [InlineData(JsonValueType.String, true)]
        [InlineData(JsonValueType.DateTime, true)]
        [InlineData(JsonValueType.Int | JsonValueType.Null, true)]
        [InlineData(JsonValueType.Float | JsonValueType.Null, true)]
        [InlineData(JsonValueType.Bool | JsonValueType.Null, true)]
        [InlineData(JsonValueType.String | JsonValueType.Null, true)]
        [InlineData(JsonValueType.DateTime | JsonValueType.Null, true)]
        [InlineData(JsonValueType.Int | JsonValueType.Enum, true)]
        [InlineData(JsonValueType.Float | JsonValueType.Enum, true)]
        [InlineData(JsonValueType.Bool | JsonValueType.Enum, true)]
        [InlineData(JsonValueType.String | JsonValueType.Enum, true)]
        [InlineData(JsonValueType.DateTime | JsonValueType.Enum, true)]
        [InlineData(JsonValueType.Object, false)]
        [InlineData(JsonValueType.Array, false)]
        [InlineData(JsonValueType.Object | JsonValueType.Null, false)]
        [InlineData(JsonValueType.Array | JsonValueType.Null, false)]
        internal void IsJsonValue_WithType_ReturnsCorrectResult(JsonValueType value, bool expected)
        {
            bool result = JsonTools.IsJsonValue(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(JsonArrayData))]
        internal void JsonArrayToType_WithValidValues_ReturnsArray(IJsonArray data, Type arrayType, Type valueType, IEnumerable<int> expected)
        {
            object result = JsonTools.JsonArrayToType(data, arrayType, valueType);

            string message = $"Expected type {arrayType.Name} is not compatible with returned type {result.GetType().Name}";
            Assert.True(arrayType.IsAssignableFrom(result.GetType()), message);
            Assert.Equal(expected, result as IEnumerable<int>);
        }

        public static readonly object[][] JsonArrayData =
        {
            new object[] { new SingleLinkedList<int> { 1, 2, 3 }, typeof(int[]), typeof(int), new int[] { 1, 2, 3 }  },
            new object[] { new SingleLinkedList<int> { 1, 2, 3 }, typeof(IEnumerable<int>), typeof(int), new List<int> { 1, 2, 3 }  },
            new object[] { new SingleLinkedList<int> { 1, 2, 3 }, typeof(IList<int>), typeof(int), new List<int> { 1, 2, 3 }  },
            new object[] { new SingleLinkedList<int> { 1, 2, 3 }, typeof(ICollection<int>), typeof(int), new List<int> { 1, 2, 3 }  },
            new object[] { new SingleLinkedList<int> { 1, 2, 3 }, typeof(List<int>), typeof(int), new List<int> { 1, 2, 3 }  },
        };
    }
}

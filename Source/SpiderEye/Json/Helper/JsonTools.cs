using System;
using System.Collections;
using System.Collections.Generic;
using SpiderEye.Json.Collections;

namespace SpiderEye.Json
{
    internal class JsonTools
    {
        private static readonly Dictionary<Type, JsonValueType> TypeToJsonMap = new Dictionary<Type, JsonValueType>
        {
            { typeof(byte), JsonValueType.Int },
            { typeof(sbyte), JsonValueType.Int },
            { typeof(short), JsonValueType.Int },
            { typeof(ushort), JsonValueType.Int },
            { typeof(int), JsonValueType.Int },
            { typeof(uint), JsonValueType.Int },
            { typeof(long), JsonValueType.Int },
            { typeof(ulong), JsonValueType.Int },
            { typeof(float), JsonValueType.Float },
            { typeof(double), JsonValueType.Float },
            { typeof(decimal), JsonValueType.Float },
            { typeof(bool), JsonValueType.Bool },
            { typeof(string), JsonValueType.String },
            { typeof(DateTime), JsonValueType.DateTime },
        };

        public static JsonValueType GetJsonType(Type type)
        {
            var nullabelType = Nullable.GetUnderlyingType(type);
            if (nullabelType != null) { type = nullabelType; }

            if (TypeToJsonMap.TryGetValue(type, out JsonValueType result)) { return result; }

            if (type.IsEnum) { return JsonValueType.Enum; }

            if (type.IsArray) { return JsonValueType.Array; }

            if (type.IsGenericType)
            {
                var gtype = type.GetGenericTypeDefinition();
                if (gtype == typeof(IEnumerable<>) ||
                    gtype == typeof(IList<>) ||
                    gtype == typeof(ICollection<>) ||
                    gtype == typeof(List<>))
                {
                    return JsonValueType.Array;
                }
            }

            return JsonValueType.Object;
        }

        public static Type GetArrayValueType(Type type)
        {
            if (type.IsArray) { return type.GetElementType(); }
            else { return type.GetGenericArguments()[0]; }
        }

        public static object JsonArrayToType(IJsonArray data, Type arrayType, Type valueType)
        {
            if (arrayType.IsArray)
            {
                var result = Array.CreateInstance(valueType, data.Count);
                int index = 0;
                foreach (object item in data) { result.SetValue(item, index++); }

                return result;
            }

            if (arrayType.IsGenericType)
            {
                var gtype = arrayType.GetGenericTypeDefinition();
                if (gtype == typeof(IEnumerable<>)) { return data; }

                if (gtype == typeof(IList<>) ||
                    gtype == typeof(ICollection<>) ||
                    gtype == typeof(List<>))
                {
                    var genericType = typeof(List<>).MakeGenericType(valueType);
                    var result = Activator.CreateInstance(genericType, data.Count) as IList;
                    foreach (object item in data) { result.Add(item); }

                    return result;
                }
            }

            throw new Exception("Could not create object for JSON array. This is a bug.");
        }

        public static bool IsJsonValue(JsonValueType type)
        {
            const JsonValueType values = JsonValueType.Object | JsonValueType.Array;
            return (values & type) == 0;
        }
    }
}

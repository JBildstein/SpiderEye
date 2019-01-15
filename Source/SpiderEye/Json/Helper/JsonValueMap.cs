using System;
using System.Reflection;
using SpiderEye.Tools;

namespace SpiderEye.Json
{
    internal sealed class JsonValueMap
    {
        public string Name { get; }
        public Type ValueType { get; }
        public Func<object, object> Getter { get; }
        public Action<object, object> Setter { get; }
        public JsonValueType JsonType { get; }
        public bool CanBeNull { get; }
        public bool AsRawJson { get; }

        public JsonValueMap(PropertyInfo info)
            : this(info, info.PropertyType)
        {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }

            if (info.GetMethod != null) { Getter = o => info.GetValue(o); }
            if (info.SetMethod != null) { Setter = (o, v) => info.SetValue(o, v); }
        }

        public JsonValueMap(FieldInfo info)
            : this(info, info.FieldType)
        {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }

            Getter = o => info.GetValue(o);
            Setter = (o, v) => info.SetValue(o, v);
        }

        private JsonValueMap(MemberInfo info, Type valueType)
        {
            Name = JsTools.NormalizeToDotnetName(info.Name);
            ValueType = valueType;
            CanBeNull = valueType.IsByRef || Nullable.GetUnderlyingType(valueType) != null;
            JsonType = JsonTools.GetJsonType(valueType);

            if (CanBeNull) { JsonType |= JsonValueType.Null; }

            if (valueType == typeof(string))
            {
                AsRawJson = info.GetCustomAttribute<RawJsonAttribute>(false) != null;
            }
        }

        public static bool IsValid(PropertyInfo info)
        {
            return info.GetMethod != null || info.SetMethod != null;
        }

        public static bool IsValid(FieldInfo info)
        {
            return !info.IsLiteral;
        }
    }
}

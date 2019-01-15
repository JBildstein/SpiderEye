using System;
using System.Collections;
using System.Globalization;
using System.Text;
using SpiderEye.Tools;

namespace SpiderEye.Json
{
    internal partial class JsonConverter
    {
        public string Serialize(object value)
        {
            if (value == null) { return "null"; }

            var type = value.GetType();
            cache.BuildMapFor(type);

            var builder = new StringBuilder();
            WriteValue(value, builder, type);

            return builder.ToString();
        }

        private void WriteValue(object value, StringBuilder builder, Type type)
        {
            if (value == null)
            {
                builder.Append("null");
                return;
            }

            var typeMap = cache.GetMap(type);

            if (typeMap.JsonType.HasFlag(JsonValueType.Object))
            {
                builder.Append("{");

                bool first = true;
                foreach (var item in typeMap.Values)
                {
                    if (item.Getter != null)
                    {
                        if (!first) { builder.Append(","); }
                        first = false;

                        builder.Append("\"");
                        builder.Append(JsTools.NormalizeToJsName(item.Name));
                        builder.Append("\":");

                        object itemValue = item.Getter(value);
                        if (item.AsRawJson && itemValue != null) { builder.Append(itemValue.ToString()); }
                        else { WriteValue(itemValue, builder, item.ValueType); }
                    }
                }

                builder.Append("}");
            }
            else if (typeMap.JsonType.HasFlag(JsonValueType.Array))
            {
                if (value is IEnumerable list)
                {
                    builder.Append("[");

                    var valueType = JsonTools.GetArrayValueType(typeMap.Type);
                    bool first = true;
                    foreach (object item in list)
                    {
                        if (!first) { builder.Append(","); }
                        first = false;

                        WriteValue(item, builder, valueType);
                    }

                    builder.Append("]");
                }
                else { throw new InvalidOperationException($"Supposed JSON array type does not implement IEnumerable: \"{type.Name}\""); }
            }
            else
            {
                switch (typeMap.JsonType & ~JsonValueType.Null)
                {
                    case JsonValueType.String:
                        WriteStringValue(value.ToString(), builder);
                        break;

                    case JsonValueType.Int:
                        long intValue = (long)Convert.ChangeType(value, typeof(long));
                        builder.Append(intValue.ToString());
                        break;

                    case JsonValueType.Float:
                        decimal floatValue = (decimal)Convert.ChangeType(value, typeof(decimal));
                        builder.Append(floatValue.ToString("G17", CultureInfo.InvariantCulture));
                        break;

                    case JsonValueType.Bool:
                        builder.Append((bool)value ? "true" : "false");
                        break;

                    case JsonValueType.DateTime:
                        var dateTimeValue = (DateTime)value;
                        builder.Append("\"");
                        builder.Append(dateTimeValue.ToString("o"));
                        if (dateTimeValue.Kind == DateTimeKind.Unspecified) { builder.Append("Z"); }
                        builder.Append("\"");
                        break;
                }
            }
        }

        private unsafe void WriteStringValue(string value, StringBuilder builder)
        {
            builder.Append("\"");

            fixed (char* ptr = value)
            {
                char* start = ptr;
                int lastIndex = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    if (ptr[i] == '\\' || ptr[i] == '"')
                    {
                        if (lastIndex != i) { builder.Append(start, i - lastIndex); }

                        builder.Append("\\");
                        builder.Append(ptr[i]);

                        lastIndex = i + 1;
                        start = ptr + lastIndex;
                    }
                    else if (ptr[i] < 32)
                    {
                        if (lastIndex != i) { builder.Append(start, i - lastIndex); }

                        builder.Append("\\u");
                        string code = Convert.ToString(ptr[i], 16);
                        builder.Append('0', 4 - code.Length);
                        builder.Append(code);

                        lastIndex = i + 1;
                        start = ptr + lastIndex;
                    }
                }

                if (lastIndex < value.Length - 1)
                {
                    builder.Append(start, value.Length - lastIndex);
                }
            }

            builder.Append("\"");
        }
    }
}

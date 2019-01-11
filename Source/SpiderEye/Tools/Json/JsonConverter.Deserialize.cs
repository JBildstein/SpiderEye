using System;
using System.Globalization;

namespace SpiderEye.Tools.Json
{
    internal static partial class JsonConverter
    {
        public static unsafe T Deserialize<T>(string json)
        {
            if (json == null) { throw new ArgumentNullException(nameof(json)); }
            if (string.IsNullOrWhiteSpace(json)) { return default; }

            return (T)Deserialize(json, typeof(T));
        }

        public static unsafe object Deserialize(string json, Type type)
        {
            if (json == null) { throw new ArgumentNullException(nameof(json)); }
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (string.IsNullOrWhiteSpace(json)) { return null; }

            JsonTypeMap.BuildMapFor(type);

            fixed (char* jsonPtr = json)
            {
                return Parse(new JsonData(jsonPtr, json.Length), type);
            }
        }

        private static object Parse(JsonData json, Type type)
        {
            var map = JsonTypeMap.GetMap(type);
            while (true)
            {
                switch (json.Value)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        break;

                    case '{':
                        CheckIsObject(map.JsonType);
                        return ParseObject(json, map);

                    case '[':
                        CheckIsArray(map.JsonType);
                        return ParseArray(json, map);

                    default:
                        return ParseValue(json, map);
                }

                json.Increment();
            }
        }

        private static unsafe object ParseObject(JsonData json, JsonTypeMap typeMap)
        {
            object result = typeMap.CreateInstance();
            while (true)
            {
                json.Increment();

                switch (json.Value)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        continue;

                    case '"':
                        string key = ParseString(json, true);
                        if (key.Length == 0) { throw new FormatException($"Invalid empty key at index {json.Index}"); }
                        json.Increment();
                        SkipKeyEndWhitespace(json);

                        JsonValueMap valueMap = typeMap.GetValueMap(key);
                        if (valueMap != null && valueMap.Setter != null)
                        {
                            object value;
                            if (valueMap.AsRawJson)
                            {
                                char* start = json.Pointer;
                                SkipValue(json);
                                int length = (int)(json.Pointer - start) + 1;
                                json.CheckCanMovePosition(1);
                                value = new string(start, 0, length);
                            }
                            else { value = Parse(json, valueMap.ValueType); }

                            if (value == null && !valueMap.CanBeNull)
                            {
                                string message = $"Cannot set null value to {typeMap.Type.Name}.{valueMap.Name}";
                                throw new InvalidOperationException(message);
                            }

                            valueMap.Setter(result, value);

                            SkipValueEndWhitespace(json);
                        }
                        else { SkipValue(json); }
                        break;

                    case '}':
                        return result;

                    default:
                        throw new FormatException($"Invalid character \"{json.GetDisplayValue()}\" at index {json.Index}");
                }
            }
        }

        private static object ParseArray(JsonData json, JsonTypeMap typeMap)
        {
            var valueType = JsonTools.GetArrayValueType(typeMap.Type);
            var valueTypeMap = JsonTypeMap.GetMap(valueType);
            var data = typeMap.CreateInstance() as IJsonArray;

            while (true)
            {
                json.Increment();

                switch (json.Value)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        continue;

                    case '{':
                        CheckIsObject(valueTypeMap.JsonType);
                        data.Add(ParseObject(json, valueTypeMap));
                        break;

                    case '[':
                        CheckIsArray(valueTypeMap.JsonType);
                        data.Add(ParseArray(json, valueTypeMap));
                        break;

                    case ']':
                        return JsonTools.JsonArrayToType(data, typeMap.Type, valueType);

                    default:
                        data.Add(ParseValue(json, valueTypeMap));
                        break;
                }
            }
        }

        private static object ParseValue(JsonData json, JsonTypeMap typeMap)
        {
            object result;
            switch (json.Value)
            {
                case '"':
                    CheckIsString(typeMap.JsonType);
                    string resultString = ParseString(json, false);
                    if (typeMap.JsonType.HasFlag(JsonValueType.DateTime))
                    {
                        result = DateTime.ParseExact(resultString, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    }
                    else { result = resultString; }
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    CheckIsNumber(typeMap.JsonType);
                    result = ParseNumber(json, typeMap);
                    break;

                case 'n':
                    ParseConstantValueString(json, "null");
                    result = null;
                    break;

                case 't':
                    CheckIsBool(typeMap.JsonType);
                    ParseConstantValueString(json, "true");
                    result = false;
                    break;

                case 'f':
                    CheckIsBool(typeMap.JsonType);
                    ParseConstantValueString(json, "false");
                    result = false;
                    break;

                default:
                    throw new FormatException($"Invalid character in value \"{json.GetDisplayValue()}\" at index {json.Index}");
            }


            return result;
        }

        private static unsafe string ParseString(JsonData json, bool normalize)
        {
            char* start = json.Pointer + 1;
            int length = GetStringLength(json, out bool hasEscapedValues);

            if (length == 0) { return string.Empty; }
            else if (length == 1)
            {
                return (normalize ? char.ToUpper(*start) : *start).ToString();
            }
            else if (!hasEscapedValues)
            {
                string value = new string(start, 0, length);
                if (normalize) { return JsTools.NormalizeToDotnetName(value); }
                else { return value; }
            }

            char* result = stackalloc char[length];
            bool escaped = false;
            for (int i = 0; i < length; i++)
            {
                switch (*start)
                {
                    case '"':
                        escaped = !escaped;
                        result[i] = *start;
                        break;

                    case '\\':
                        if (escaped) { result[i] = '\\'; }
                        escaped = !escaped;
                        break;

                    case '/':
                        escaped = false;
                        goto default;

                    case 'b':
                        if (escaped)
                        {
                            result[i] = '\b';
                            escaped = false;
                            break;
                        }
                        else { goto default; }

                    case 'f':
                        if (escaped)
                        {
                            result[i] = '\f';
                            escaped = false;
                            break;
                        }
                        else { goto default; }

                    case 'n':
                        if (escaped)
                        {
                            result[i] = '\n';
                            escaped = false;
                            break;
                        }
                        else { goto default; }

                    case 'r':
                        if (escaped)
                        {
                            result[i] = '\r';
                            escaped = false;
                            break;
                        }
                        else { goto default; }

                    case 't':
                        if (escaped)
                        {
                            result[i] = '\t';
                            escaped = false;
                            break;
                        }
                        else { goto default; }

                    case 'u':
                        if (escaped)
                        {
                            string codeString = new string(json.Pointer, 0, 4);
                            result[i] = (char)Convert.ToInt32(codeString, 16);
                            escaped = false;
                            break;
                        }
                        else { goto default; }

                    default:
                        result[i] = *start;
                        break;
                }

                start++;
            }

            if (normalize) { result[0] = char.ToUpper(result[0]); }

            return new string(result, 0, length);
        }

        private static int GetStringLength(JsonData json, out bool hasEscapedValues)
        {
            int length = 0;
            bool escaped = false;
            hasEscapedValues = false;
            while (true)
            {
                json.Increment();

                switch (json.Value)
                {
                    case '"':
                        if (!escaped) { return length; }
                        else { escaped = false; }
                        break;

                    case '\\':
                        escaped = !escaped;
                        hasEscapedValues = true;
                        if (escaped) { continue; }
                        break;

                    case '/':
                    case 'b':
                    case 'f':
                    case 'n':
                    case 'r':
                    case 't':
                        escaped = false;
                        break;

                    case 'u':
                        if (escaped) { VerifyEscapedUnicode(json); }
                        escaped = false;
                        break;

                    default:
                        if (json.Value < 32) { throw new FormatException($"Invalid character \"{json.GetDisplayValue()}\" at index {json.Index}"); }
                        if (escaped) { throw new FormatException($"Invalid escaped character \"{json.GetDisplayValue()}\" at index {json.Index}"); }
                        break;
                }

                length++;
            }
        }

        private static object ParseNumber(JsonData json, JsonTypeMap typeMap)
        {
            string stringValue = GetNumberString(json, out bool isFloat);
            if (isFloat)
            {
                decimal result = decimal.Parse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture);
                if (typeMap.JsonType.HasFlag(JsonValueType.DateTime))
                {
                    // interpret value as Unix seconds
                    return DateTimeOffset.FromUnixTimeMilliseconds((long)((result * 1000) + 0.5m)).DateTime;
                }
                else { return Convert.ChangeType(result, typeMap.Type); }
            }
            else
            {
                long result = long.Parse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
                if (typeMap.JsonType.HasFlag(JsonValueType.DateTime))
                {
                    // interpret value as Unix milliseconds
                    return DateTimeOffset.FromUnixTimeMilliseconds(result).DateTime;
                }
                else { return Convert.ChangeType(result, typeMap.Type); }
            }
        }

        private static unsafe string GetNumberString(JsonData json, out bool isFloat)
        {
            char* start = json.Pointer;
            int length = 1;
            isFloat = false;
            while (true)
            {
                json.Increment();

                switch (json.Value)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                    case ',':
                        return new string(start, 0, length);

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                    case 'e':
                    case 'E':
                        // valid values
                        break;

                    case '.':
                        isFloat = true;
                        break;

                    default:
                        throw new FormatException($"Invalid character in number \"{json.GetDisplayValue()}\" at index {json.Index}");
                }

                length++;
            }
        }

        private static void ParseConstantValueString(JsonData json, string expected)
        {
            for (int i = 1; i < expected.Length; i++)
            {
                json.Increment();

                if (json.Value != expected[i])
                {
                    throw new FormatException($"Invalid character in \"{expected}\" value \"{json.GetDisplayValue()}\" at index {json.Index}");
                }
            }
        }

        private static void VerifyEscapedUnicode(JsonData json)
        {
            for (int i = 0; i < 4; i++)
            {
                json.Increment();
                bool isHexChar = (json.Value >= '0' && json.Value <= '9')
                    || (json.Value >= 'a' && json.Value <= 'f')
                    || (json.Value >= 'A' && json.Value <= 'F');

                if (!isHexChar)
                {
                    throw new FormatException($"Invalid character in Unicode escaped value \"{json.GetDisplayValue()}\" at index {json.Index}");
                }
            }
        }

        private static void SkipValueEndWhitespace(JsonData json)
        {
            json.Increment();

            while (true)
            {
                switch (json.Value)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        json.Increment();
                        continue;

                    case '}':
                    case ']':
                        json.Decrement();
                        return;

                    case ',':
                        return;

                    default:
                        throw new FormatException($"Invalid character \"{json.GetDisplayValue()}\" at index {json.Index}");
                }
            }
        }

        private static void SkipKeyEndWhitespace(JsonData json)
        {
            while (true)
            {
                switch (json.Value)
                {
                    case ' ':
                    case '\t':
                        json.Increment();
                        continue;

                    case ':':
                        json.Increment();
                        return;

                    default:
                        throw new FormatException($"Invalid character \"{json.GetDisplayValue()}\" at index {json.Index}");
                }
            }
        }

        private static void SkipValue(JsonData json)
        {
            int objectDepth = 0;
            int arrayDepth = 0;
            while (true)
            {
                switch (json.Value)
                {
                    case '"':
                        GetStringLength(json, out _);
                        break;

                    case '{':
                        objectDepth++;
                        break;

                    case '}':
                        objectDepth--;
                        if (objectDepth < 0)
                        {
                            if (arrayDepth != 0) { throw new FormatException($"Invalid closing object character \"}}\" at index {json.Index}"); }
                            json.Decrement();
                            return;
                        }

                        break;

                    case '[':
                        arrayDepth++;
                        break;

                    case ']':
                        arrayDepth--;
                        if (arrayDepth < 0)
                        {
                            if (objectDepth != 0) { throw new FormatException($"Invalid closing array character \"]\" at index {json.Index}"); }
                            json.Decrement();
                            return;
                        }

                        break;

                    case ',':
                        if (arrayDepth == 0 && objectDepth == 0) { return; }
                        break;
                }

                json.Increment();
            }
        }

        private static void CheckIsObject(JsonValueType type)
        {
            if (!type.HasFlag(JsonValueType.Object)) { ThrowWrongJsonTypeError("object", type); }
        }

        private static void CheckIsArray(JsonValueType type)
        {
            if (!type.HasFlag(JsonValueType.Array)) { ThrowWrongJsonTypeError("array", type); }
        }

        private static void CheckIsBool(JsonValueType type)
        {
            if (!type.HasFlag(JsonValueType.Bool)) { ThrowWrongJsonTypeError("boolean", type); }
        }

        private static void CheckIsString(JsonValueType type)
        {
            if (!type.HasFlag(JsonValueType.String) && !type.HasFlag(JsonValueType.DateTime))
            {
                ThrowWrongJsonTypeError("string", type);
            }
        }

        private static void CheckIsNumber(JsonValueType type)
        {
            if (!JsonTools.IsJsonNumber(type) && !type.HasFlag(JsonValueType.DateTime))
            {
                ThrowWrongJsonTypeError("number", type);
            }
        }

        private static void CheckIsValue(JsonValueType type)
        {
            if (!JsonTools.IsJsonValue(type)) { ThrowWrongJsonTypeError("value", type); }
        }

        private static void ThrowWrongJsonTypeError(string actual, JsonValueType expected)
        {
            throw new Exception($"JSON defines {actual} but expected a JSON type of {expected}");
        }
    }
}

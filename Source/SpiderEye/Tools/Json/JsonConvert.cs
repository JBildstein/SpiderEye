using System;

namespace SpiderEye.Tools.Json
{
    internal static class JsonConvert
    {
        public static IJsonConverter DefaultConverter = new JsonConverter();

        public static T Deserialize<T>(string json)
        {
            return DefaultConverter.Deserialize<T>(json);
        }

        public static object Deserialize(string json, Type type)
        {
            return DefaultConverter.Deserialize(json, type);
        }

        public static string Serialize(object value)
        {
            return DefaultConverter.Serialize(value);
        }
    }
}

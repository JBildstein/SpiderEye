using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiderEye.Json
{
    internal sealed class JsonTypeMap
    {
        public Type Type { get; }
        public JsonValueType JsonType { get; }
        public IEnumerable<JsonValueMap> Values
        {
            get { return valueMaps?.Values ?? Enumerable.Empty<JsonValueMap>(); }
        }

        private readonly Func<object> constructor;
        private readonly Dictionary<string, JsonValueMap> valueMaps;

        public JsonTypeMap(Type type, JsonValueType jsonType)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            JsonType = jsonType;
        }

        public JsonTypeMap(Type type, JsonValueType jsonType, Func<object> constructor)
            : this(type, jsonType)
        {
            this.constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        public JsonTypeMap(Type type, JsonValueType jsonType, Func<object> constructor, Dictionary<string, JsonValueMap> valueMaps)
            : this(type, jsonType, constructor)
        {
            this.valueMaps = valueMaps ?? throw new ArgumentNullException(nameof(valueMaps));
        }

        public object CreateInstance()
        {
            if (constructor == null) { throw new InvalidOperationException($"Cannot create instance of type {Type.Name}. This is a bug."); }

            return constructor();
        }

        public JsonValueMap GetValueMap(string name)
        {
            if (valueMaps == null) { throw new InvalidOperationException($"Type {Type.Name} does not have value maps. This is a bug."); }

            if (valueMaps.TryGetValue(name, out JsonValueMap map)) { return map; }
            else { return null; }
        }
    }
}

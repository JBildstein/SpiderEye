using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpiderEye.Tools.Json
{
    internal class JsonTypeMap
    {
        public Type Type { get; }
        public JsonValueType JsonType { get; }
        public IEnumerable<JsonValueMap> Values
        {
            get { return valueMaps?.Values ?? Enumerable.Empty<JsonValueMap>(); }
        }

        private static readonly Dictionary<Type, JsonTypeMap> AllMaps = new Dictionary<Type, JsonTypeMap>();

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

        public JsonTypeMap(Type type, JsonValueType jsonType, ConstructorInfo constructor, Dictionary<string, JsonValueMap> valueMaps)
            : this(type, jsonType)
        {
            if (constructor == null) { throw new ArgumentNullException(nameof(constructor)); }

            this.constructor = () => constructor.Invoke(null);
            this.valueMaps = valueMaps ?? throw new ArgumentNullException(nameof(valueMaps));
        }

        public static void BuildMapFor<T>()
        {
            BuildMapFor(typeof(T));
        }

        public static void BuildMapFor(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (type == typeof(object)) { throw new NotSupportedException("Cannot map values to type object"); }

            if (AllMaps.ContainsKey(type)) { return; }

            var jsonType = JsonTools.GetJsonType(type);
            if (jsonType == JsonValueType.Object)
            {
                var ctor = type.GetConstructors().FirstOrDefault(t => t.GetParameters().Length == 0);
                if (ctor == null) { throw new InvalidOperationException($"Type {type.Name} does not have a suitable constructor"); }

                var valueMaps = new Dictionary<string, JsonValueMap>();
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!IsIgnored(property) && JsonValueMap.IsValid(property))
                    {
                        var map = new JsonValueMap(property);
                        valueMaps.Add(map.Name, map);

                        BuildMapFor(property.PropertyType);
                    }
                }

                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!IsIgnored(field) && JsonValueMap.IsValid(field))
                    {
                        var map = new JsonValueMap(field);
                        valueMaps.Add(map.Name, map);

                        BuildMapFor(field.FieldType);
                    }
                }

                AllMaps.Add(type, new JsonTypeMap(type, jsonType, ctor, valueMaps));
            }
            else if (jsonType == JsonValueType.Array)
            {
                var valueType = JsonTools.GetArrayValueType(type);
                var genericType = typeof(LinkedList<>).MakeGenericType(valueType);

                Func<object> ctor = () => Activator.CreateInstance(genericType);
                AllMaps.Add(type, new JsonTypeMap(type, jsonType, ctor));
            }
            else if (JsonTools.IsJsonValue(jsonType))
            {
                AllMaps.Add(type, new JsonTypeMap(type, jsonType));
            }
        }

        public static JsonTypeMap GetMap(Type type)
        {
            if (!AllMaps.TryGetValue(type, out JsonTypeMap map))
            {
                throw new Exception($"No type map found for Type {type.Name}. This is a bug.");
            }

            return map;
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

        private static bool IsIgnored(MemberInfo info)
        {
            var ignoreAttribute = typeof(JsonIgnoreAttribute);
            foreach (var attribute in info.CustomAttributes)
            {
                if (attribute.AttributeType == ignoreAttribute)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

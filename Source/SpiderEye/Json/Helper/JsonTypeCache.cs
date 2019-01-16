using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SpiderEye.Json.Collections;

namespace SpiderEye.Json
{
    internal sealed class JsonTypeCache
    {
        private readonly Dictionary<Type, JsonTypeMap> maps = new Dictionary<Type, JsonTypeMap>();
        private readonly object lockObject = new object();

        public void BuildMapFor<T>()
        {
            BuildMapFor(typeof(T));
        }

        public void BuildMapFor(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (type == typeof(object)) { throw new NotSupportedException("Cannot map values to type object"); }

            lock (lockObject)
            {
                if (maps.ContainsKey(type)) { return; }

                var jsonType = JsonTools.GetJsonType(type);
                if (jsonType == JsonValueType.Object)
                {
                    Func<object> constructor;
                    if (type.IsValueType) { constructor = () => Activator.CreateInstance(type); }
                    else
                    {
                        var constructorInfo = type.GetConstructors().FirstOrDefault(t => t.GetParameters().Length == 0);
                        if (constructorInfo == null) { throw new InvalidOperationException($"Type {type.Name} does not have a suitable constructor"); }

                        constructor = () => constructorInfo.Invoke(null);
                    }

                    var valueMaps = new Dictionary<string, JsonValueMap>();
                    maps.Add(type, new JsonTypeMap(type, jsonType, constructor, valueMaps));

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
                }
                else if (jsonType == JsonValueType.Array)
                {
                    var valueType = JsonTools.GetArrayValueType(type);
                    var genericType = typeof(SingleLinkedList<>).MakeGenericType(valueType);

                    Func<object> constructor = () => Activator.CreateInstance(genericType);
                    maps.Add(type, new JsonTypeMap(type, jsonType, constructor));

                    BuildMapFor(valueType);
                }
                else if (JsonTools.IsJsonValue(jsonType))
                {
                    maps.Add(type, new JsonTypeMap(type, jsonType));
                }
                else { throw new Exception($"Unknown JSON value type \"{jsonType}\". This is a bug."); }
            }
        }

        public JsonTypeMap GetMap(Type type)
        {
            lock (lockObject)
            {
                if (!maps.TryGetValue(type, out JsonTypeMap map))
                {
                    throw new Exception($"No type map found for Type {type.Name}. This is a bug.");
                }

                return map;
            }
        }

        private static bool IsIgnored(MemberInfo info)
        {
            return info.GetCustomAttribute<JsonIgnoreAttribute>(false) != null;
        }
    }
}

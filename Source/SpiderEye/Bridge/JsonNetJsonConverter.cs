using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SpiderEye.Bridge
{
    internal class JsonNetJsonConverter : IJsonConverter
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    // needed for the JRaw backing fields, they use nameof(Property)
                    OverrideSpecifiedNames = true,
                },
            },
        };

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, Settings);
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }
    }
}

using System;

namespace SpiderEye.Json
{
    internal partial class JsonConverter : IJsonConverter
    {
        private readonly JsonTypeCache cache;

        public JsonConverter()
            : this(new JsonTypeCache())
        {
        }

        public JsonConverter(JsonTypeCache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
    }
}

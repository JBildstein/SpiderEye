using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpiderEye.Bridge.Models
{
    internal class EventResultModel
    {
        [JsonIgnore]
        public string? Result
        {
            get { return ResultRaw?.Value as string; }
            set { ResultRaw = value == null ? null : new JRaw(value); }
        }

        public bool HasResult { get; set; }
        public JsErrorModel? Error { get; set; }
        public bool Success { get; set; }
        public bool NoSubscriber { get; set; }

        [JsonProperty(nameof(Result))]
        private JRaw? ResultRaw { get; set; }
    }
}

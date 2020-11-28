using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpiderEye.Bridge.Models
{
    internal class InvokeInfoModel
    {
        public string? Type { get; set; }
        public string? Id { get; set; }
        public int? CallbackId { get; set; }

        [JsonIgnore]
        public string? Parameters
        {
            get { return ParametersRaw?.Value as string; }
            set { ParametersRaw = value == null ? null : new JRaw(value); }
        }

        [JsonProperty(nameof(Parameters))]
        private JRaw? ParametersRaw { get; set; }
    }
}

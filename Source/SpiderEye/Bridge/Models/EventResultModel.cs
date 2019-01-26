using SpiderEye.Json;

namespace SpiderEye.Bridge.Models
{
    internal class EventResultModel
    {
        [RawJson]
        public string Result { get; set; }
        public bool HasResult { get; set; }
        public string Error { get; set; }
        public bool Success { get; set; }
    }
}

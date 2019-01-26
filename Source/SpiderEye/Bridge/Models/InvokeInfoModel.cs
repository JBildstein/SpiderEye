using SpiderEye.Json;

namespace SpiderEye.Bridge.Models
{
    internal class InvokeInfoModel
    {
        public string Type { get; set; }
        public string Id { get; set; }
        [RawJson]
        public string Parameters { get; set; }
        public int? CallbackId { get; set; }
    }
}

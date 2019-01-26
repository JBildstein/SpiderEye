using SpiderEye.Json;

namespace SpiderEye.Bridge.Models
{
    internal class ApiResultModel
    {
        [RawJson]
        public string Value { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public static ApiResultModel FromError(string message)
        {
            return new ApiResultModel
            {
                Value = null,
                Success = false,
                Error = message,
            };
        }
    }
}

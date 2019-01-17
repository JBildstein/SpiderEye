using System;
using SpiderEye.Json;
using SpiderEye.Scripting.Api;
using SpiderEye.Scripting.Models;
using SpiderEye.UI;

namespace SpiderEye.Scripting
{
    internal class ScriptHandler
    {
        public event EventHandler<string> TitleChanged;

        private readonly IWebview webview;

        public ScriptHandler(IWebview webview)
        {
            this.webview = webview ?? throw new ArgumentNullException(nameof(webview));
        }

        public void HandleScriptCall(string data)
        {
            try
            {
                var info = JsonConvert.Deserialize<InvokeInfoModel>(data);
                if (info != null)
                {
                    if (info.Type == "title")
                    {
                        string title = JsonConvert.Deserialize<string>(info.Parameters);
                        TitleChanged?.Invoke(this, title);
                    }
                    else if (info.Type == "api")
                    {
                        var result = ApiResolver.ResolveCall(info.Id, info.Parameters);
                        EndApiCall(info, result);
                    }
                    else if (info.CallbackId != null)
                    {
                        string message = $"Invalid invoke type \"{info.Type ?? "<null>"}\"";
                        EndApiCall(info, ApiResultModel.FromError(message));
                    }
                }
            }
            catch { /* TODO: do something with this */ }
        }

        private void EndApiCall(InvokeInfoModel info, ApiResultModel result)
        {
            string resultJson = JsonConvert.Serialize(result);
            webview.ExecuteScript($"window._spidereye._endApiCall({info.CallbackId}, {resultJson})");
        }
    }
}

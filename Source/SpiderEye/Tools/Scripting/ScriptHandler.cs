using System;
using SpiderEye.Tools.Json;
using SpiderEye.Tools.Scripting.Api;
using SpiderEye.Tools.Scripting.Models;

namespace SpiderEye.Tools.Scripting
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
                var info = JsonConverter.Deserialize<InvokeInfoModel>(data);
                if (info != null)
                {
                    if (info.Type == "title")
                    {
                        string title = JsonConverter.Deserialize<string>(info.Parameters);
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
            string resultJson = JsonConverter.Serialize(result);
            webview.ExecuteScript($"window._spidereye._endApiCall({info.CallbackId}, {resultJson})");
        }
    }
}

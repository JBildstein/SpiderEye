using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SpiderEye.Bridge.Api;
using SpiderEye.Bridge.Models;
using SpiderEye.Json;
using SpiderEye.UI;

namespace SpiderEye.Bridge
{
    internal class WebviewBridge : IWebviewBridge
    {
        public event EventHandler<string> TitleChanged;

        private static readonly object GlobalHandlerLock = new object();
        private static readonly List<object> GlobalHandler = new List<object>();

        private readonly HashSet<string> apiRootNames = new HashSet<string>();
        private readonly Dictionary<string, ApiMethod> apiMethods = new Dictionary<string, ApiMethod>();

        private IWindow window;
        private IWebview webview;
        private IWindowFactory windowFactory;

        public void Init(IWindow window, IWebview webview, IWindowFactory windowFactory)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            this.webview = webview ?? throw new ArgumentNullException(nameof(webview));
            this.windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));

            InitApi();
        }

        public void AddHandler(object handler)
        {
            AddApiObject(handler);
        }

        public void AddGlobalHandler(object handler)
        {
            AddHandler(handler);
            lock (GlobalHandlerLock) { GlobalHandler.Add(handler); }
        }

        public void Invoke(string id, object data)
        {
            string script = GetInvokeScript(id, data);
            string resultJson = webview.ExecuteScript(script);
            ResolveEventResult(resultJson);
        }

        public async Task InvokeAsync(string id, object data)
        {
            string script = GetInvokeScript(id, data);
            string resultJson = await webview.ExecuteScriptAsync(script);
            ResolveEventResult(resultJson);
        }

        public T Invoke<T>(string id, object data)
        {
            string script = GetInvokeScript(id, data);
            string resultJson = webview.ExecuteScript(script);
            var result = ResolveEventResult(resultJson);
            return ResolveInvokeResult<T>(result);
        }

        public async Task<T> InvokeAsync<T>(string id, object data)
        {
            string script = GetInvokeScript(id, data);
            string resultJson = await webview.ExecuteScriptAsync(script);
            var result = ResolveEventResult(resultJson);
            return ResolveInvokeResult<T>(result);
        }

        public async Task HandleScriptCall(string data)
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
                    var result = await ResolveCall(info.Id, info.Parameters);
                    EndApiCall(info, result);
                }
                else if (info.CallbackId != null)
                {
                    string message = $"Invalid invoke type \"{info.Type ?? "<null>"}\".";
                    EndApiCall(info, ApiResultModel.FromError(message));
                }
            }
        }

        private string GetInvokeScript(string id, object data)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException(nameof(id)); }

            string dataJson = JsonConvert.Serialize(data);
            return $"window._spidereye._sendEvent({id}, {dataJson})";
        }

        private EventResultModel ResolveEventResult(string resultJson)
        {
            var result = JsonConvert.Deserialize<EventResultModel>(resultJson);

            // TODO: include error info from result
            if (!result.Success) { throw new ScriptException(); }

            return result;
        }

        private T ResolveInvokeResult<T>(EventResultModel result)
        {
            if (!result.HasResult) { return default; }
            else { return JsonConvert.Deserialize<T>(result.Result); }
        }

        private void EndApiCall(InvokeInfoModel info, ApiResultModel result)
        {
            string resultJson = JsonConvert.Serialize(result);
            webview.ExecuteScript($"window._spidereye._endApiCall({info.CallbackId}, {resultJson})");
        }

        private async Task<ApiResultModel> ResolveCall(string id, string parameters)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return ApiResultModel.FromError("No API name given.");
            }

            if (apiMethods.TryGetValue(id, out ApiMethod info))
            {
                try
                {
                    object parametersObject = null;
                    if (info.HasParameter && !string.IsNullOrWhiteSpace(parameters))
                    {
                        parametersObject = JsonConvert.Deserialize(parameters, info.ParameterType);
                    }

                    object result = await info.InvokeAsync(parametersObject);
                    return new ApiResultModel
                    {
                        Success = true,
                        Value = info.HasReturnValue ? JsonConvert.Serialize(result) : null,
                    };
                }
                catch (Exception ex) { return ApiResultModel.FromError(ex.Message); }
            }
            else { return ApiResultModel.FromError($"Unknown API call \"{id}\"."); }
        }

        private void AddApiObject(object instance)
        {
            Type type = instance.GetType();
            string rootName = type.Name;
            if (!apiRootNames.Add(rootName))
            {
                throw new InvalidOperationException($"Handler with name \"{rootName}\" already exists.");
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.GetParameters().Length > 1) { continue; }

                var info = new ApiMethod(instance, method);
                string fullName = $"{rootName}.{info.Name}";

                if (apiMethods.ContainsKey(fullName))
                {
                    throw new InvalidOperationException($"Method with name \"{fullName}\" already exists.");
                }

                apiMethods.Add(fullName, info);
            }
        }

        private void InitApi()
        {
            AddApiObject(new Dialog(window, windowFactory));

            lock (GlobalHandlerLock)
            {
                foreach (object handler in GlobalHandler)
                {
                    AddApiObject(handler);
                }
            }
        }
    }
}

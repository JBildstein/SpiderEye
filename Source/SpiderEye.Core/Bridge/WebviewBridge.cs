using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using SpiderEye.Bridge.Api;
using SpiderEye.Bridge.Models;

namespace SpiderEye.Bridge
{
    internal class WebviewBridge : IWebviewBridge
    {
        public event EventHandler<string>? TitleChanged;

        private IWebview Webview
        {
            get { return window.NativeWindow.Webview; }
        }

        private static event EventHandler<object>? GlobalEventHandlerUpdate;
        private static readonly object GlobalHandlerLock = new();
        private static readonly List<object> GlobalHandler = new();

        private static readonly IJsonConverter JsonConverter = new JsonNetJsonConverter();

        private readonly HashSet<string> apiRootNames = new();
        private readonly Dictionary<string, ApiMethod> apiMethods = new();

        private readonly Window window;

        public WebviewBridge(Window window)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));

            InitApi();
        }

        public static void AddGlobalHandlerStatic(object handler)
        {
            lock (GlobalHandlerLock)
            {
                GlobalHandler.Add(handler);
                GlobalEventHandlerUpdate?.Invoke(null, handler);
            }
        }

        public void AddHandler(object handler)
        {
            AddApiObject(handler);
        }

        public void AddGlobalHandler(object handler)
        {
            AddGlobalHandlerStatic(handler);
        }

        public async Task InvokeAsync(string id, object data)
        {
            string script = GetInvokeScript(id, data);
            string? resultJson = await Application.Invoke(() => Webview.ExecuteScriptAsync(script));
            ResolveEventResult(id, resultJson);
        }

        public async Task<T?> InvokeAsync<T>(string id, object data)
        {
            string script = GetInvokeScript(id, data);
            string? resultJson = await Application.Invoke(() => Webview.ExecuteScriptAsync(script));
            var result = ResolveEventResult(id, resultJson);
            return ResolveInvokeResult<T>(result);
        }

        public async Task HandleScriptCall(string data)
        {
            // run script call handling on separate task to free up UI
            await Task.Run(async () =>
            {
                try
                {
                    var info = JsonConverter.Deserialize<InvokeInfoModel>(data);
                    if (info != null)
                    {
                        if (info.Type == "title")
                        {
                            string title;
                            if (info.Parameters == null) { title = string.Empty; }
                            else { title = JsonConverter.Deserialize<string>(info.Parameters) ?? string.Empty; }

                            TitleChanged?.Invoke(this, title);
                        }
                        else if (info.Type == "api")
                        {
                            var result = await ResolveCall(info.Id, info.Parameters);
                            await EndApiCall(info, result);
                        }
                        else if (info.CallbackId != null)
                        {
                            string message = $"Invalid invoke type \"{info.Type ?? "<null>"}\".";
                            await EndApiCall(info, ApiResultModel.FromError(message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // An Exception may happen if the window that started the request got closed before the result came back.
                    // In any case, errors at this point should not crash the whole application and don't really matter.
                    Debug.WriteLine(ex.ToString());
                }
            });
        }

        private static string GetInvokeScript(string id, object data)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException(nameof(id)); }

            string dataJson = JsonConverter.Serialize(data);
            string idJson = JsonConverter.Serialize(id); // this makes sure that the name is properly escaped
            return $"window._spidereye._sendEvent({idJson}, {dataJson})";
        }

        private static EventResultModel ResolveEventResult(string id, string? resultJson)
        {
            if (resultJson == null) { throw new InvalidOperationException($"Event with ID \"{id}\" did not return result JSON."); }

            var result = JsonConverter.Deserialize<EventResultModel>(resultJson);

            if (result == null) { throw new InvalidOperationException($"Event with ID \"{id}\" did not return result."); }
            if (result.NoSubscriber) { throw new InvalidOperationException($"Event with ID \"{id}\" does not exist."); }

            if (!result.Success)
            {
                string? message = result.Error?.Message;
                if (string.IsNullOrWhiteSpace(message)) { message = $"Error executing Event with ID \"{id}\"."; }
                else if (!string.IsNullOrWhiteSpace(result.Error?.Name)) { message = $"{result.Error.Name}: {message}"; }

                string? stackTrace = result.Error?.Stack;
                if (string.IsNullOrWhiteSpace(stackTrace)) { throw new ScriptException(message); }
                else { throw new ScriptException(message, new ScriptException(message, stackTrace)); }
            }

            return result;
        }

        private static T? ResolveInvokeResult<T>(EventResultModel result)
        {
            if (!result.HasResult || result.Result == null) { return default; }
            else { return JsonConverter.Deserialize<T>(result.Result); }
        }

        private async Task EndApiCall(InvokeInfoModel info, ApiResultModel result)
        {
            string resultJson = JsonConverter.Serialize(result);
            string script = $"window._spidereye._endApiCall({info.CallbackId}, {resultJson})";
            await Application.Invoke(() => Webview.ExecuteScriptAsync(script));
        }

        private async Task<ApiResultModel> ResolveCall(string? id, string? parameters)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return ApiResultModel.FromError("No API name given.");
            }

            if (apiMethods.TryGetValue(id, out var info))
            {
                try
                {
                    object? parametersObject = null;
                    if (info.HasParameter && !string.IsNullOrWhiteSpace(parameters))
                    {
                        parametersObject = JsonConverter.Deserialize(parameters, info.ParameterType);
                    }

                    object? result = await info.InvokeAsync(parametersObject);
                    return new ApiResultModel
                    {
                        Success = true,
                        Value = info.HasReturnValue ? JsonConverter.Serialize(result) : null,
                    };
                }
                catch (TargetInvocationException tex) { return ApiResultModel.FromError(tex.InnerException ?? tex); }
                catch (Exception ex) { return ApiResultModel.FromError(ex); }
            }
            else { return ApiResultModel.FromError($"Unknown API call \"{id}\"."); }
        }

        private void AddApiObject(object handler)
        {
            if (handler == null) { throw new ArgumentNullException(nameof(handler)); }

            Type type = handler.GetType();
            string rootName = type.Name;
            var attribute = type.GetCustomAttribute<BridgeObjectAttribute>();
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Path)) { rootName = attribute.Path; }

            if (!apiRootNames.Add(rootName))
            {
                throw new InvalidOperationException($"Handler with name \"{rootName}\" already exists.");
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.DeclaringType == typeof(object)) { continue; }
                if (method.GetParameters().Length > 1) { continue; }

                var info = new ApiMethod(handler, method);
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
            AddApiObject(new WindowApiBridge(window));
            AddApiObject(new DialogApiBridge(window));

            lock (GlobalHandlerLock)
            {
                GlobalEventHandlerUpdate += (s, e) => AddApiObject(e);

                foreach (object handler in GlobalHandler)
                {
                    AddApiObject(handler);
                }
            }
        }
    }
}

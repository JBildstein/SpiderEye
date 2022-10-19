using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SpiderEye.Bridge;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaWebview : IWebview
    {
        public event NavigatingEventHandler? Navigating;
        public event PageLoadEventHandler? PageLoaded;

        public event EventHandler<string>? TitleChanged;

        public bool EnableScriptInterface { get; set; }
        public bool UseBrowserTitle { get; set; }
        public bool EnableDevTools
        {
            get { return enableDevToolsField; }
            set
            {
                enableDevToolsField = value;
                IntPtr boolValue = Foundation.Call("NSNumber", "numberWithBool:", value);
                ObjC.Call(preferences, "setValue:forKey:", boolValue, NSString.Create("developerExtrasEnabled"));
            }
        }

        private Uri Uri
        {
            get { return URL.GetAsUri(ObjC.Call(Handle, "URL"))!; }
        }

        public readonly IntPtr Handle;

        private static readonly NativeClassDefinition CallbackClassDefinition;
        private static readonly NativeClassDefinition SchemeHandlerDefinition;

        private readonly NativeClassInstance callbackClass;
        private readonly NativeClassInstance schemeHandler;

        private readonly WebviewBridge bridge;
        private readonly Uri customHost;
        private readonly IntPtr preferences;

        private bool enableDevToolsField;

        static CocoaWebview()
        {
            CallbackClassDefinition = CreateCallbackClass();
            SchemeHandlerDefinition = CreateSchemeHandler();
        }

        public CocoaWebview(WebviewBridge bridge)
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            IntPtr configuration = WebKit.Call("WKWebViewConfiguration", "new");
            IntPtr manager = ObjC.Call(configuration, "userContentController");

            callbackClass = CallbackClassDefinition.CreateInstance(this);
            schemeHandler = SchemeHandlerDefinition.CreateInstance(this);

            const string scheme = "spidereye";
            customHost = new Uri(UriTools.GetRandomResourceUrl(scheme));
            ObjC.Call(configuration, "setURLSchemeHandler:forURLScheme:", schemeHandler.Handle, NSString.Create(scheme));

            ObjC.Call(manager, "addScriptMessageHandler:name:", callbackClass.Handle, NSString.Create("external"));
            IntPtr script = WebKit.Call("WKUserScript", "alloc");
            ObjC.Call(
                script,
                "initWithSource:injectionTime:forMainFrameOnly:",
                NSString.Create(Resources.GetInitScript("Mac")),
                IntPtr.Zero,
                IntPtr.Zero);
            ObjC.Call(manager, "addUserScript:", script);

            Handle = WebKit.Call("WKWebView", "alloc");
            ObjC.Call(Handle, "initWithFrame:configuration:", CGRect.Zero, configuration);
            ObjC.Call(Handle, "setNavigationDelegate:", callbackClass.Handle);

            IntPtr boolValue = Foundation.Call("NSNumber", "numberWithBool:", false);
            ObjC.Call(Handle, "setValue:forKey:", boolValue, NSString.Create("drawsBackground"));
            ObjC.Call(Handle, "addObserver:forKeyPath:options:context:", callbackClass.Handle, NSString.Create("title"), IntPtr.Zero, IntPtr.Zero);

            preferences = ObjC.Call(configuration, "preferences");
        }

        public void UpdateBackgroundColor(IntPtr color)
        {
            ObjC.Call(Handle, "setBackgroundColor:", color);
        }

        public void LoadUri(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            if (!uri.IsAbsoluteUri) { uri = new Uri(customHost, uri); }

            IntPtr nsUrl = Foundation.Call("NSURL", "URLWithString:", NSString.Create(uri.ToString()));
            IntPtr request = Foundation.Call("NSURLRequest", "requestWithURL:", nsUrl);
            ObjC.Call(Handle, "loadRequest:", request);
        }

        public async Task<string?> ExecuteScriptAsync(string script)
        {
            var taskResult = new TaskCompletionSource<string?>();

            ScriptEvalCallbackDelegate callback = (IntPtr self, IntPtr result, IntPtr error) =>
            {
                try
                {
                    if (error != IntPtr.Zero)
                    {
                        string? message = NSString.GetString(ObjC.Call(error, "localizedDescription"));
                        taskResult.TrySetException(new ScriptException($"Script execution failed with: \"{message}\""));
                    }
                    else
                    {
                        string? content = NSString.GetString(result);
                        taskResult.TrySetResult(content);
                    }
                }
                catch (Exception ex) { taskResult.TrySetException(ex); }
            };

            using NSBlock block = new NSBlock(callback);
            ObjC.Call(
                Handle,
                "evaluateJavaScript:completionHandler:",
                NSString.Create(script),
                block.Handle);

            return await taskResult.Task;
        }

        public void Dispose()
        {
            // webview will be released automatically
            callbackClass.Dispose();
            schemeHandler.Dispose();
        }

        private static NativeClassDefinition CreateCallbackClass()
        {
            // note: WKScriptMessageHandler is not available at runtime and returns null, it's kept for completeness
            var definition = NativeClassDefinition.FromObject(
                "SpiderEyeWebviewCallbacks",
                WebKit.GetProtocol("WKNavigationDelegate"),
                WebKit.GetProtocol("WKScriptMessageHandler"));

            definition.AddMethod<NavigationDecideDelegate>(
                "webView:decidePolicyForNavigationAction:decisionHandler:",
                "v@:@@@",
                (self, op, view, navigationAction, decisionHandler) =>
                {
                    var instance = definition.GetParent<CocoaWebview>(self);
                    var args = new NavigatingEventArgs(instance.Uri);
                    instance.Navigating?.Invoke(instance, args);

                    var block = Marshal.PtrToStructure<NSBlock.BlockLiteral>(decisionHandler);
                    var callback = Marshal.GetDelegateForFunctionPointer<NavigationDecisionDelegate>(block.Invoke);
                    callback(decisionHandler, args.Cancel ? IntPtr.Zero : new IntPtr(1));
                });

            definition.AddMethod<LoadFinishedDelegate>(
                "webView:didFinishNavigation:",
                "v@:@@",
                (self, op, view, navigation) =>
                {
                    var instance = definition.GetParent<CocoaWebview>(self);
                    instance.PageLoaded?.Invoke(instance, new PageLoadEventArgs(instance.Uri, true));
                });

            definition.AddMethod<LoadFailedDelegate>(
                "webView:didFailNavigation:withError:",
                "v@:@@@",
                (self, op, view, navigation, error) =>
                {
                    var instance = definition.GetParent<CocoaWebview>(self);
                    instance.PageLoaded?.Invoke(instance, new PageLoadEventArgs(instance.Uri, false));
                });

            definition.AddMethod<ObserveValueDelegate>(
                "observeValueForKeyPath:ofObject:change:context:",
                "v@:@@@@",
                (self, op, keyPath, obj, change, context) =>
                {
                    var instance = definition.GetParent<CocoaWebview>(self);
                    ObservedValueChanged(instance, keyPath);
                });

            definition.AddMethod<ScriptCallbackDelegate>(
                "userContentController:didReceiveScriptMessage:",
                "v@:@@",
                (self, op, notification, message) =>
                {
                    var instance = definition.GetParent<CocoaWebview>(self);
                    ScriptCallback(instance, message);
                });

            definition.FinishDeclaration();

            return definition;
        }

        private static NativeClassDefinition CreateSchemeHandler()
        {
            // note: WKURLSchemeHandler is not available at runtime and returns null, it's kept for completeness
            var definition = NativeClassDefinition.FromObject(
                "SpiderEyeSchemeHandler",
                WebKit.GetProtocol("WKURLSchemeHandler"));

            definition.AddMethod<SchemeHandlerDelegate>(
                "webView:startURLSchemeTask:",
                "v@:@@",
                (self, op, view, schemeTask) =>
                {
                    var instance = definition.GetParent<CocoaWebview>(self);
                    UriSchemeStartCallback(instance, schemeTask);
                });

            definition.AddMethod<SchemeHandlerDelegate>(
                "webView:stopURLSchemeTask:",
                "v@:@@",
                (self, op, view, schemeTask) => { /* don't think anything needs to be done here */ });

            definition.FinishDeclaration();

            return definition;
        }

        private static void ObservedValueChanged(CocoaWebview instance, IntPtr keyPath)
        {
            string? key = NSString.GetString(keyPath);
            if (key == "title" && instance.UseBrowserTitle)
            {
                string? title = NSString.GetString(ObjC.Call(instance.Handle, "title"));
                instance.TitleChanged?.Invoke(instance, title ?? string.Empty);
            }
        }

        private static async void ScriptCallback(CocoaWebview instance, IntPtr message)
        {
            if (instance.EnableScriptInterface)
            {
                IntPtr body = ObjC.Call(message, "body");
                IntPtr isString = ObjC.Call(body, "isKindOfClass:", Foundation.GetClass("NSString"));
                if (isString != IntPtr.Zero)
                {
                    string data = NSString.GetString(body)!;
                    await instance.bridge.HandleScriptCall(data);
                }
            }
        }

        private static void UriSchemeStartCallback(CocoaWebview instance, IntPtr schemeTask)
        {
            try
            {
                IntPtr request = ObjC.Call(schemeTask, "request");
                IntPtr url = ObjC.Call(request, "URL");

                var uri = URL.GetAsUri(url)!;
                var host = new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
                if (host == instance.customHost)
                {
                    using var contentStream = Application.ContentProvider.GetStreamAsync(uri).GetAwaiter().GetResult();
                    if (contentStream != null)
                    {
                        if (contentStream is UnmanagedMemoryStream unmanagedMemoryStream)
                        {
                            unsafe
                            {
                                long length = unmanagedMemoryStream.Length - unmanagedMemoryStream.Position;
                                var data = (IntPtr)unmanagedMemoryStream.PositionPointer;
                                FinishUriSchemeCallback(url, schemeTask, data, length, uri);
                                return;
                            }
                        }
                        else
                        {
                            byte[] data;
                            long length;
                            if (contentStream is MemoryStream memoryStream)
                            {
                                data = memoryStream.GetBuffer();
                                length = memoryStream.Length;
                            }
                            else
                            {
                                using var copyStream = new MemoryStream();
                                contentStream.CopyTo(copyStream);
                                data = copyStream.GetBuffer();
                                length = copyStream.Length;
                            }

                            unsafe
                            {
                                fixed (byte* dataPtr = data)
                                {
                                    FinishUriSchemeCallback(url, schemeTask, (IntPtr)dataPtr, length, uri);
                                    return;
                                }
                            }
                        }
                    }
                }

                FinishUriSchemeCallbackWithError(schemeTask, 404);
            }
            catch { FinishUriSchemeCallbackWithError(schemeTask, 500); }
        }

        private static void FinishUriSchemeCallback(IntPtr url, IntPtr schemeTask, IntPtr data, long contentLength, Uri uri)
        {
            IntPtr response = Foundation.Call("NSURLResponse", "alloc");
            ObjC.Call(
                response,
                "initWithURL:MIMEType:expectedContentLength:textEncodingName:",
                url,
                NSString.Create(MimeTypes.FindForUri(uri)),
                new IntPtr(contentLength),
                IntPtr.Zero);

            ObjC.Call(schemeTask, "didReceiveResponse:", response);

            IntPtr nsData = Foundation.Call(
                "NSData",
                "dataWithBytesNoCopy:length:freeWhenDone:",
                data,
                new IntPtr(contentLength),
                IntPtr.Zero);
            ObjC.Call(schemeTask, "didReceiveData:", nsData);

            ObjC.Call(schemeTask, "didFinish");
        }

        private static void FinishUriSchemeCallbackWithError(IntPtr schemeTask, int errorCode)
        {
            var error = Foundation.Call(
                "NSError",
                "errorWithDomain:code:userInfo:",
                NSString.Create("com.bildstein.spidereye"),
                new IntPtr(errorCode),
                IntPtr.Zero);
            ObjC.Call(schemeTask, "didFailWithError:", error);
        }
    }
}

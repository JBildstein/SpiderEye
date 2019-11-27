using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SpiderEye.Bridge;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaWebview : IWebview
    {
        public event PageLoadEventHandler PageLoaded;
        public event EventHandler<string> TitleChanged;

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

        public readonly IntPtr Handle;

        private static int count = 0;

        private readonly WebviewBridge bridge;
        private readonly Uri customHost;
        private readonly IntPtr preferences;

        private readonly LoadFinishedDelegate loadDelegate;
        private readonly LoadFailedDelegate loadFailedDelegate;
        private readonly ObserveValueDelegate observedValueChangedDelegate;
        private readonly ScriptCallbackDelegate scriptDelegate;
        private readonly SchemeHandlerDelegate uriSchemeStartDelegate;
        private readonly SchemeHandlerDelegate uriSchemeStopDelegate;

        private bool enableDevToolsField;

        public CocoaWebview(WebviewBridge bridge)
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            Interlocked.Increment(ref count);

            // need to keep the delegates around or they will get garbage collected
            loadDelegate = LoadCallback;
            loadFailedDelegate = LoadFailedCallback;
            observedValueChangedDelegate = ObservedValueChanged;
            scriptDelegate = ScriptCallback;
            uriSchemeStartDelegate = UriSchemeStartCallback;
            uriSchemeStopDelegate = UriSchemeStopCallback;

            IntPtr configuration = WebKit.Call("WKWebViewConfiguration", "new");
            IntPtr manager = ObjC.Call(configuration, "userContentController");
            IntPtr callbackClass = CreateCallbackClass();

            customHost = CreateSchemeHandler(configuration);

            ObjC.Call(manager, "addScriptMessageHandler:name:", callbackClass, NSString.Create("external"));
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
            ObjC.Call(Handle, "setNavigationDelegate:", callbackClass);

            IntPtr boolValue = Foundation.Call("NSNumber", "numberWithBool:", false);
            ObjC.Call(Handle, "setValue:forKey:", boolValue, NSString.Create("drawsBackground"));
            ObjC.Call(Handle, "addObserver:forKeyPath:options:context:", callbackClass, NSString.Create("title"), IntPtr.Zero, IntPtr.Zero);

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

        public Task<string> ExecuteScriptAsync(string script)
        {
            var taskResult = new TaskCompletionSource<string>();
            NSBlock block = null;

            ScriptEvalCallbackDelegate callback = (IntPtr self, IntPtr result, IntPtr error) =>
            {
                try
                {
                    if (error != IntPtr.Zero)
                    {
                        string message = NSString.GetString(ObjC.Call(error, "localizedDescription"));
                        taskResult.TrySetException(new Exception($"Script execution failed with: \"{message}\""));
                    }
                    else
                    {
                        string content = NSString.GetString(result);
                        taskResult.TrySetResult(content);
                    }
                }
                catch (Exception ex) { taskResult.TrySetException(ex); }
                finally { block.Dispose(); }
            };

            block = new NSBlock(callback);
            ObjC.Call(
                Handle,
                "evaluateJavaScript:completionHandler:",
                NSString.Create(script),
                block.Handle);

            return taskResult.Task;
        }

        public void Dispose()
        {
            // will be released automatically
        }

        private IntPtr CreateCallbackClass()
        {
            IntPtr callbackClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "CallbackClass" + count, IntPtr.Zero);
            ObjC.AddProtocol(callbackClass, ObjC.GetProtocol("WKNavigationDelegate"));
            ObjC.AddProtocol(callbackClass, ObjC.GetProtocol("WKScriptMessageHandler"));

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("webView:didFinishNavigation:"),
                loadDelegate,
                "v@:@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("webView:didFailNavigation:withError:"),
                loadFailedDelegate,
                "v@:@@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("observeValueForKeyPath:ofObject:change:context:"),
                observedValueChangedDelegate,
                "v@:@@@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("userContentController:didReceiveScriptMessage:"),
                scriptDelegate,
                "v@:@@");

            ObjC.RegisterClassPair(callbackClass);

            return ObjC.Call(callbackClass, "new");
        }

        private Uri CreateSchemeHandler(IntPtr configuration)
        {
            const string scheme = "spidereye";
            string host = UriTools.GetRandomResourceUrl(scheme);

            IntPtr handlerClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "SchemeHandler" + count, IntPtr.Zero);
            ObjC.AddProtocol(handlerClass, ObjC.GetProtocol("WKURLSchemeHandler"));

            ObjC.AddMethod(
                handlerClass,
                ObjC.RegisterName("webView:startURLSchemeTask:"),
                uriSchemeStartDelegate,
                "v@:@@");

            ObjC.AddMethod(
                handlerClass,
                ObjC.RegisterName("webView:stopURLSchemeTask:"),
                uriSchemeStopDelegate,
                "v@:@@");

            ObjC.RegisterClassPair(handlerClass);

            IntPtr handler = ObjC.Call(handlerClass, "new");
            ObjC.Call(configuration, "setURLSchemeHandler:forURLScheme:", handler, NSString.Create(scheme));

            return new Uri(host);
        }

        private async void ScriptCallback(IntPtr self, IntPtr op, IntPtr notification, IntPtr message)
        {
            if (EnableScriptInterface)
            {
                IntPtr body = ObjC.Call(message, "body");
                IntPtr isString = ObjC.Call(body, "isKindOfClass:", Foundation.GetClass("NSString"));
                if (isString != IntPtr.Zero)
                {
                    string data = NSString.GetString(body);
                    await bridge.HandleScriptCall(data);
                }
            }
        }

        private void UriSchemeStartCallback(IntPtr self, IntPtr op, IntPtr view, IntPtr schemeTask)
        {
            try
            {
                IntPtr request = ObjC.Call(schemeTask, "request");
                IntPtr url = ObjC.Call(request, "URL");

                var uri = new Uri(NSString.GetString(ObjC.Call(url, "absoluteString")));
                var host = new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
                if (host == customHost)
                {
                    using (var contentStream = Application.ContentProvider.GetStreamAsync(uri).GetAwaiter().GetResult())
                    {
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
                                    using (var copyStream = new MemoryStream())
                                    {
                                        contentStream.CopyTo(copyStream);
                                        data = copyStream.GetBuffer();
                                        length = copyStream.Length;
                                    }
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
                }

                FinishUriSchemeCallbackWithError(schemeTask, 404);
            }
            catch { FinishUriSchemeCallbackWithError(schemeTask, 500); }
        }

        private void UriSchemeStopCallback(IntPtr self, IntPtr op, IntPtr view, IntPtr schemeTask)
        {
            // don't think anything needs to be done here
        }

        private void ObservedValueChanged(IntPtr self, IntPtr op, IntPtr keyPath, IntPtr obj, IntPtr change, IntPtr context)
        {
            string key = NSString.GetString(keyPath);
            if (key == "title" && UseBrowserTitle)
            {
                string title = NSString.GetString(ObjC.Call(Handle, "title"));
                TitleChanged?.Invoke(this, title ?? string.Empty);
            }
        }

        private void LoadFailedCallback(IntPtr self, IntPtr op, IntPtr view, IntPtr navigation, IntPtr error)
        {
            PageLoaded?.Invoke(this, PageLoadEventArgs.Failed);
        }

        private void LoadCallback(IntPtr self, IntPtr op, IntPtr view, IntPtr navigation)
        {
            PageLoaded?.Invoke(this, PageLoadEventArgs.Successful);
        }

        private void FinishUriSchemeCallback(IntPtr url, IntPtr schemeTask, IntPtr data, long contentLength, Uri uri)
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

        private void FinishUriSchemeCallbackWithError(IntPtr schemeTask, int errorCode)
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

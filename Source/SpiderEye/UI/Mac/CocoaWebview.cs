using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.Tools;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaWebview : IWebview
    {
        public event PageLoadEventHandler PageLoaded;
        public event EventHandler<string> TitleChanged;

        public readonly IntPtr Handle;

        private static int count = 0;

        private readonly IContentProvider contentProvider;
        private readonly AppConfiguration config;
        private readonly WebviewBridge bridge;
        private readonly string customHost;

        private readonly bool enableDevTools = false;

        public CocoaWebview(AppConfiguration config, IContentProvider contentProvider, WebviewBridge bridge)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            Interlocked.Increment(ref count);

            IntPtr configuration = WebKit.Call("WKWebViewConfiguration", "new");
            IntPtr manager = ObjC.Call(configuration, "userContentController");
            IntPtr callbackClass = CreateCallbackClass();

            customHost = CreateSchemeHandler(configuration);

            if (config.EnableScriptInterface)
            {
                ObjC.Call(manager, "addScriptMessageHandler:name:", callbackClass, NSString.Create("external"));
                IntPtr script = WebKit.Call("WKUserScript", "alloc");
                ObjC.Call(
                    script,
                    "initWithSource:injectionTime:forMainFrameOnly:",
                    NSString.Create(Resources.GetInitScript("Unix")),
                    IntPtr.Zero,
                    IntPtr.Zero);
                ObjC.Call(manager, "addUserScript:", script);
            }

            Handle = WebKit.Call("WKWebView", "alloc");
            ObjC.Call(Handle, "initWithFrame:configuration:", CGRect.Zero, configuration);
            ObjC.Call(Handle, "setNavigationDelegate:", callbackClass);

            IntPtr bgColor = NSColor.FromHex(config.Window.BackgroundColor);
            ObjC.Call(Handle, "setBackgroundColor:", bgColor);

            IntPtr boolValue = Foundation.Call("NSNumber", "numberWithBool:", 0);
            ObjC.Call(Handle, "setValue:forKey:", boolValue, NSString.Create("drawsBackground"));

            if (config.Window.UseBrowserTitle)
            {
                ObjC.Call(Handle, "addObserver:forKeyPath:options:context:", callbackClass, NSString.Create("title"), IntPtr.Zero, IntPtr.Zero);
            }

            if (enableDevTools)
            {
                var preferences = ObjC.Call(configuration, "preferences");
                ObjC.Call(preferences, "setValue:forKey:", new IntPtr(1), NSString.Create("developerExtrasEnabled"));
            }
        }

        public void NavigateToFile(string url)
        {
            if (url == null) { throw new ArgumentNullException(nameof(url)); }

            if (customHost != null) { url = UriTools.Combine(customHost, url).ToString(); }
            else { url = UriTools.Combine(config.ExternalHost, url).ToString(); }

            IntPtr nsUrl = Foundation.Call("NSURL", "URLWithString:", NSString.Create(url));
            IntPtr request = Foundation.Call("NSURLRequest", "requestWithURL:", nsUrl);
            ObjC.Call(Handle, "loadRequest:", request);
        }

        public string ExecuteScript(string script)
        {
            return NSRunLoop.WaitForTask(ExecuteScriptAsync(script));
        }

        public Task<string> ExecuteScriptAsync(string script)
        {
            var taskResult = new TaskCompletionSource<string>();
            NSBlock block = null;

            unsafe void Callback(IntPtr self, IntPtr result, IntPtr error)
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
            }

            block = new NSBlock((ScriptEvalCallbackDelegate)Callback);
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
                (LoadFinishedDelegate)LoadCallback,
                "v@:@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("webView:didFailNavigation:withError:"),
                (LoadFailedDelegate)LoadFailedCallback,
                "v@:@@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("observeValueForKeyPath:ofObject:change:context:"),
                (ObserveValueDelegate)ObservedValueChanged,
                "v@:@@@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("userContentController:didReceiveScriptMessage:"),
                (ScriptCallbackDelegate)ScriptCallback,
                "v@:@@");

            ObjC.RegisterClassPair(callbackClass);

            return ObjC.Call(callbackClass, "new");
        }

        private string CreateSchemeHandler(IntPtr configuration)
        {
            string host = null;
            if (string.IsNullOrWhiteSpace(config.ExternalHost))
            {
                const string scheme = "spidereye";
                host = UriTools.GetRandomResourceUrl(scheme);

                IntPtr handlerClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "SchemeHandler" + count, IntPtr.Zero);
                ObjC.AddProtocol(handlerClass, ObjC.GetProtocol("WKURLSchemeHandler"));

                ObjC.AddMethod(
                    handlerClass,
                    ObjC.RegisterName("webView:startURLSchemeTask:"),
                    (SchemeHandlerDelegate)UriSchemeStartCallback,
                    "v@:@@");

                ObjC.AddMethod(
                    handlerClass,
                    ObjC.RegisterName("webView:stopURLSchemeTask:"),
                    (SchemeHandlerDelegate)UriSchemeStopCallback,
                    "v@:@@");

                ObjC.RegisterClassPair(handlerClass);

                IntPtr handler = ObjC.Call(handlerClass, "new");
                ObjC.Call(configuration, "setURLSchemeHandler:forURLScheme:", handler, NSString.Create(scheme));
            }

            return host;
        }

        private async void ScriptCallback(IntPtr self, IntPtr op, IntPtr notification, IntPtr message)
        {
            IntPtr body = ObjC.Call(message, "body");
            IntPtr isString = ObjC.Call(body, "isKindOfClass:", Foundation.GetClass("NSString"));
            if (isString != IntPtr.Zero)
            {
                string data = NSString.GetString(body);
                await bridge.HandleScriptCall(data);
            }
        }

        private void UriSchemeStartCallback(IntPtr self, IntPtr op, IntPtr view, IntPtr schemeTask)
        {
            try
            {
                IntPtr request = ObjC.Call(schemeTask, "request");
                IntPtr url = ObjC.Call(request, "URL");

                var uri = new Uri(NSString.GetString(ObjC.Call(url, "absoluteString")));
                string schemeAndServer = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                if (schemeAndServer == customHost)
                {
                    using (var contentStream = contentProvider.GetStreamAsync(uri).GetAwaiter().GetResult())
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
            if (key == "title")
            {
                string title = NSString.GetString(ObjC.Call(Handle, "title"));
                TitleChanged?.Invoke(this, title);
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

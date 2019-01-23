using System;
using System.IO;
using System.Threading.Tasks;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.Scripting;
using SpiderEye.Tools;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaWebview : IWebview
    {
        public event EventHandler<string> TitleChanged;

        public ScriptHandler ScriptHandler { get; }

        public readonly IntPtr Handle;

        private readonly IContentProvider contentProvider;
        private readonly AppConfiguration config;

        private string customHost;

        public CocoaWebview(IContentProvider contentProvider, AppConfiguration config)
        {
            this.contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            IntPtr configuration = WebKit.Call("WKWebViewConfiguration", "new");
            IntPtr manager = WebKit.Call(configuration, "userContentController");

            CreateSchemeHandler(configuration);

            Handle = WebKit.Call("WKWebView", "alloc");
            ObjC.Call(Handle, "initWithFrame:configuration:", CGRect.Zero, configuration);

            if (config.EnableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);
            }

            IntPtr bgColor = NSColor.FromHex(config.Window.BackgroundColor);
            ObjC.Call(Handle, "setBackgroundColor:", bgColor);

            IntPtr boolValue = Foundation.Call("NSNumber", "numberWithBool:", 0);
            NSString.Use("drawsBackground", nsString => ObjC.Call(Handle, "setValue:forKey:", boolValue, nsString));

            CreateCallbackClass();
        }

        public void NavigateToFile(string url)
        {
            if (url == null) { throw new ArgumentNullException(nameof(url)); }

            if (customHost != null) { url = UriTools.Combine(customHost, url).ToString(); }
            else { url = UriTools.Combine(config.ExternalHost, url).ToString(); }

            NSString.Use(url, nsUrlString =>
            {
                IntPtr nsUrl = Foundation.Call("NSURL", "URLWithString:", nsUrlString);
                IntPtr request = Foundation.Call("NSURLRequest", "requestWithURL:", nsUrl);
                ObjC.Call(Handle, "loadRequest:", request);
            });
        }

        public string ExecuteScript(string script)
        {
            var task = ExecuteScriptAsync(script);

            IntPtr loop = Foundation.Call("NSRunLoop", "currentRunLoop");
            IntPtr mode = Foundation.Call(loop, "currentMode");
            IntPtr date = Foundation.Call("NSDate", "distantFuture");

            // main loop would deadlock without this
            while (!task.IsCompleted)
            {
                ObjC.Call(loop, "runMode:beforeDate:", mode, date);
            }

            return task.Result;
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

            block = NSBlock.Create<ScriptEvalCallbackDelegate>(Callback);
            NSString.Use(script, nsString =>
            {
                ObjC.Call(
                    Handle,
                    "evaluateJavaScript:completionHandler:",
                    nsString,
                    block.Handle);
            });

            return taskResult.Task;
        }

        public void Dispose()
        {
            // will be released automatically
        }

        private void CreateCallbackClass()
        {
            IntPtr callbackClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "CallbackClass", IntPtr.Zero);
            ObjC.AddProtocol(callbackClass, ObjC.GetProtocol("WKNavigationDelegate"));
            ObjC.AddProtocol(callbackClass, ObjC.GetProtocol("WKScriptMessageHandler"));

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("webView:didFinishNavigation:"),
                (LoadFinishedDelegate)LoadCallback,
                "v@:@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("observeValueForKeyPath:ofObject:change:context:"),
                (ObserveValueDelegate)ObservedValueChanged,
                "v@:@@@@");

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("userContentController:didReceiveScriptMessage:"),
                (ScriptCallbackDelegate)((s, o, n, m) =>
                {
                    Console.WriteLine("Script message received");
                }),
                "v@:@@");

            ObjC.RegisterClassPair(callbackClass);

            IntPtr callbacks = ObjC.Call(callbackClass, "new");
            ObjC.Call(Handle, "setNavigationDelegate:", callbacks);

            if (config.Window.UseBrowserTitle)
            {
                NSString.Use("title", nsString =>
                {
                    ObjC.Call(Handle, "addObserver:forKeyPath:options:context:", callbacks, nsString, IntPtr.Zero, IntPtr.Zero);
                });
            }
        }

        private void CreateSchemeHandler(IntPtr configuration)
        {
            if (string.IsNullOrWhiteSpace(config.ExternalHost))
            {
                const string scheme = "spidereye";
                customHost = UriTools.GetRandomResourceUrl(scheme);

                IntPtr handlerClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "SchemeHandler", IntPtr.Zero);
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
                NSString.Use(scheme, nsString => ObjC.Call(configuration, "setURLSchemeHandler:forURLScheme:", handler, nsString));
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
                if (false && schemeAndServer == customHost)
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

        private void LoadCallback(IntPtr self, IntPtr op, IntPtr view, IntPtr navigation)
        {
        }

        private void FinishUriSchemeCallback(IntPtr url, IntPtr schemeTask, IntPtr data, long contentLength, Uri uri)
        {
            IntPtr response = Foundation.Call("NSURLResponse", "alloc");
            NSString.Use(MimeTypes.FindForUri(uri), nsString =>
            {
                ObjC.Call(
                    response,
                    "initWithURL:MIMEType:expectedContentLength:textEncodingName:",
                    url,
                    nsString,
                    new IntPtr(contentLength),
                    IntPtr.Zero);
            });

            ObjC.Call(schemeTask, "didReceiveResponse:", response);

            IntPtr nsData = Foundation.Call("NSData", "dataWithBytesNoCopy:length:freeWhenDone:", data, new IntPtr(contentLength), IntPtr.Zero);
            ObjC.Call(schemeTask, "didReceiveData:", nsData);

            ObjC.Call(schemeTask, "didFinish");
        }

        private void FinishUriSchemeCallbackWithError(IntPtr schemeTask, int errorCode)
        {
            IntPtr error = IntPtr.Zero;
            NSString.Use("com.bildstein.spidereye", domain =>
            {
                error = Foundation.Call("NSError", "errorWithDomain:code:userInfo:", domain, new IntPtr(errorCode), IntPtr.Zero);
            });

            ObjC.Call(schemeTask, "didFailWithError:", error);
        }
    }
}

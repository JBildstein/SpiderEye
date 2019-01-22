using System;
using System.Threading.Tasks;
using SpiderEye.Configuration;
using SpiderEye.Scripting;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaWebview : IWebview
    {
        public event EventHandler<string> TitleChanged;

        public ScriptHandler ScriptHandler { get; }

        public readonly IntPtr Handle;

        private readonly AppConfiguration config;

        public CocoaWebview(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            IntPtr conf = WebKit.Call("WKWebViewConfiguration", "new");
            IntPtr manager = WebKit.Call(conf, "userContentController");

            Handle = WebKit.Call("WKWebView", "alloc");
            ObjC.Call(Handle, "initWithFrame:configuration:", CGRect.Zero, conf);

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
    }
}

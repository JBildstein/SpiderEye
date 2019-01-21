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
            // TODO: execute JavaScript
            return null;
        }

        public Task<string> ExecuteScriptAsync(string script)
        {
            // TODO: execute JavaScript
            return Task.FromResult<string>(null);
        }

        public void Dispose()
        {
            // will be released automatically
        }
    }
}

using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using SpiderEye.Bridge;
using SpiderEye.Tools;
using WebKit;

namespace SpiderEye.Mac
{
    internal class CocoaWebview : WKWebView, IWebview
    {
        public event NavigatingEventHandler? Navigating;
        public event PageLoadEventHandler? PageLoaded;

        public event EventHandler<string>? TitleChanged;

        public bool EnableScriptInterface { get; set; }
        public bool UseBrowserTitle { get; set; }
        public bool EnableDevTools
        {
            get
            {
                using var key = new NSString("developerExtrasEnabled");
                using var value = (NSNumber)preferences.ValueForKey(key);
                return value.BoolValue;
            }
            set
            {
                using var boolValue = new NSNumber(value);
                using var key = new NSString("developerExtrasEnabled");
                preferences.SetValueForKey(boolValue, key);
            }
        }

        private Uri Uri
        {
            get { return this.Url; }
        }

        private const string SCHEME = "spidereye";
        private readonly WebviewBridge bridge;
        private readonly Uri customHost;
        private readonly WKPreferences preferences;
        private readonly IDisposable titleObserver;

        public CocoaWebview(WebviewBridge bridge)
            : base(CGRect.Empty, CreateConfiguration())
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            customHost = new Uri(UriTools.GetRandomResourceUrl(SCHEME));

            WKUserContentController manager = Configuration.UserContentController;
            manager.AddScriptMessageHandler(new CocoaScriptMessageHandler(this), "external");
            using var initScriptSource = new NSString(Resources.GetInitScript("Mac"));
            var script = new WKUserScript(initScriptSource, WKUserScriptInjectionTime.AtDocumentStart, false);
            manager.AddUserScript(script);

            NavigationDelegate = new CocoaNavigationDelegate();

            using var boolValue = NSNumber.FromBoolean(false);
            using var key = new NSString("drawsBackground");
            SetValueForKey(boolValue, key);

            titleObserver = AddObserver("title", NSKeyValueObservingOptions.New, observedChange =>
            {
                if (UseBrowserTitle)
                {
                    TitleChanged?.Invoke(this, (NSString?)observedChange.NewValue ?? string.Empty);
                }
            });

            preferences = Configuration.Preferences;
        }

        public void LoadUri(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            if (!uri.IsAbsoluteUri) { uri = new Uri(customHost, uri); }

            using var request = NSUrlRequest.FromUrl(uri);
            LoadRequest(request);
        }

        public Task<string?> ExecuteScriptAsync(string script)
        {
            var taskResult = new TaskCompletionSource<string?>();
            EvaluateJavaScript(script, (result, error) =>
            {
                if (error != null)
                {
                    taskResult.SetException(new ScriptException("Script execution failed.", new NSErrorException(error)));
                }
                else
                {
                    taskResult.SetResult((NSString)result);
                }
            });
            return taskResult.Task;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                titleObserver.Dispose();
            }

            base.Dispose(disposing);
        }

        private static WKWebViewConfiguration CreateConfiguration()
        {
            var configuration = new WKWebViewConfiguration();
            configuration.SetUrlSchemeHandler(new CocoaUrlSchemeHandler(), SCHEME);
            return configuration;
        }

        private class CocoaNavigationDelegate : WKNavigationDelegate
        {
            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                var cocoaWebView = (CocoaWebview)webView;
                var args = new NavigatingEventArgs(cocoaWebView.Uri);
                cocoaWebView.Navigating?.Invoke(cocoaWebView, args);
                decisionHandler.Invoke(args.Cancel ? WKNavigationActionPolicy.Cancel : WKNavigationActionPolicy.Allow);
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                var cocoaWebView = (CocoaWebview)webView;
                cocoaWebView.PageLoaded?.Invoke(cocoaWebView, new PageLoadEventArgs(cocoaWebView.Uri, true));
            }

            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                var cocoaWebView = (CocoaWebview)webView;
                cocoaWebView.PageLoaded?.Invoke(cocoaWebView, new PageLoadEventArgs(cocoaWebView.Uri, false));
            }
        }

        private class CocoaScriptMessageHandler : WKScriptMessageHandler
        {
            private readonly CocoaWebview cocoaWebView;

            public CocoaScriptMessageHandler(CocoaWebview cocoaWebView)
            {
                this.cocoaWebView = cocoaWebView;
            }

            public override async void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                if (cocoaWebView.EnableScriptInterface && message.Body is NSString body)
                {
                    await cocoaWebView.bridge.HandleScriptCall(body);
                }
            }
        }

        private class CocoaUrlSchemeHandler : NSObject, IWKUrlSchemeHandler
        {
            public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                try
                {
                    var cocoaWebView = (CocoaWebview)webView;
                    var request = urlSchemeTask.Request;
                    Uri uri = request.Url;

                    var host = new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
                    if (host == cocoaWebView.customHost)
                    {
                        using var contentStream = Application.ContentProvider.GetStreamAsync(uri).GetAwaiter().GetResult();
                        if (contentStream != null)
                        {
                            using var data = NSData.FromStream(contentStream)!;
                            using var response = new NSUrlResponse(uri, MimeTypes.FindForUri(uri), (nint)contentStream.Length, null);
                            urlSchemeTask.DidReceiveResponse(response);
                            urlSchemeTask.DidReceiveData(data);
                            urlSchemeTask.DidFinish();
                            return;
                        }
                    }

                    FinishUriSchemeCallbackWithError(urlSchemeTask, 404);
                }
                catch { FinishUriSchemeCallbackWithError(urlSchemeTask, 500); }
            }

            public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                // don't think anything needs to be done here
            }

            private static void FinishUriSchemeCallbackWithError(IWKUrlSchemeTask urlSchemeTask, int errorCode)
            {
                using var domain = new NSString("com.bildstein.spidereye");
                using var error = new NSError(domain, errorCode);
                urlSchemeTask.DidFailWithError(error);
            }
        }
    }
}

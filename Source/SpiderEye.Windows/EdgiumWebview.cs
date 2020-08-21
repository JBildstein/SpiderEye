using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SpiderEye.Bridge;
using SpiderEye.Tools;

namespace SpiderEye.Windows
{
    /// <summary>
    /// Chromium Edge based webview.
    /// </summary>
    internal class EdgiumWebview : IWinFormsWebview
    {
        public event NavigatingEventHandler Navigating;
        public event PageLoadEventHandler PageLoaded;

        public event EventHandler<string> TitleChanged;

        public Control Control
        {
            get { return webview; }
        }

        public bool EnableScriptInterface
        {
            get { return webview.CoreWebView2.Settings.IsWebMessageEnabled; }
            set { webview.CoreWebView2.Settings.IsWebMessageEnabled = value; }
        }

        public bool EnableDevTools { get; set; }

        private readonly WebviewBridge bridge;
        private readonly WebView2 webview;
        private readonly Uri customHost;
        private readonly Task initTask;

        private CoreWebView2Environment environment;
        private Uri lastNavigatedUri;

        public EdgiumWebview(WebviewBridge bridge)
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            const string scheme = "http";
            customHost = new Uri(UriTools.GetRandomResourceUrl(scheme));

            webview = new WebView2();
            webview.NavigationStarting += Webview_NavigationStarting;
            webview.NavigationCompleted += Webview_NavigationCompleted;
            webview.WebMessageReceived += Webview_WebMessageReceived;

            InitWebview().RunSyncWithPump();
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await webview.ExecuteScriptAsync(script);
        }

        public void LoadUri(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            if (!uri.IsAbsoluteUri) { uri = new Uri(customHost, uri); }

            webview.Source = uri;
        }

        public void UpdateBackgroundColor(byte r, byte g, byte b)
        {
            webview.BackColor = System.Drawing.Color.FromArgb(r, g, b);
        }

        public void Dispose()
        {
            webview.Dispose();
        }

        private async Task InitWebview()
        {
            environment = await CoreWebView2Environment.CreateAsync();
            await webview.EnsureCoreWebView2Async(environment);

            webview.CoreWebView2.AddWebResourceRequestedFilter(customHost.ToString() + "*", CoreWebView2WebResourceContext.All);
            webview.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            webview.CoreWebView2.DocumentTitleChanged += (s, t) => { TitleChanged?.Invoke(this, webview.CoreWebView2.DocumentTitle); };

            string initScript = Resources.GetInitScript("Edgium");
            await webview.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(initScript);
        }

        private async void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();

            try
            {
                using (var contentStream = await Application.ContentProvider.GetStreamAsync(e.Request.RequestUri))
                {
                    if (contentStream != null)
                    {
                        e.Response = environment.CreateWebResourceResponse(contentStream, 200, "OK", string.Empty);
                    }
                    else { e.Response = environment.CreateWebResourceResponse(null, 404, "Not Found", string.Empty); }
                }
            }
            catch { e.Response = environment.CreateWebResourceResponse(null, 500, "Internal Server Error", string.Empty); }
            finally { deferral.Complete(); }
        }

        private void Webview_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var uri = lastNavigatedUri = new Uri(e.Uri);
            var args = new NavigatingEventArgs(uri);
            Navigating?.Invoke(this, args);
            e.Cancel = args.Cancel;
        }

        private void Webview_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (EnableDevTools) { webview.CoreWebView2.OpenDevToolsWindow(); }

            PageLoaded?.Invoke(this, new PageLoadEventArgs(lastNavigatedUri, e.IsSuccess));
        }

        private async void Webview_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            await bridge.HandleScriptCall(e.TryGetWebMessageAsString());
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SpiderEye.Bridge;

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
        private readonly Uri hostAddress;

        private CoreWebView2Environment environment;
        private Uri lastNavigatedUri;

        public EdgiumWebview(string hostAddress, WebviewBridge bridge)
        {
            if (hostAddress == null) { throw new ArgumentNullException(nameof(hostAddress)); }
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            this.hostAddress = new Uri(hostAddress, UriKind.Absolute);

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

            if (!uri.IsAbsoluteUri) { uri = new Uri(hostAddress, uri); }

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

            webview.CoreWebView2.DocumentTitleChanged += (s, t) => { TitleChanged?.Invoke(this, webview.CoreWebView2.DocumentTitle); };

            string initScript = Resources.GetInitScript("Edgium");
            await webview.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(initScript);
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

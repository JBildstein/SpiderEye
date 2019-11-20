using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiderEye.Bridge;
using SpiderEye.Content;
using SpiderEye.Tools;
using Windows.Foundation;
using Windows.Web.UI;
using Windows.Web.UI.Interop;
using Color = Windows.UI.Color;

namespace SpiderEye.UI.Windows
{
    internal class WinFormsWebview : Control, IWebview, IWinFormsWebview
    {
        public event PageLoadEventHandler PageLoaded;

        public Control Control
        {
            get { return this; }
        }

        private readonly WindowConfiguration config;
        private readonly WebviewBridge bridge;
        private readonly EdgeUriToStreamResolver streamResolver;

        private WebViewControl webview;

        public WinFormsWebview(WindowConfiguration config, IContentProvider contentProvider, WebviewBridge bridge)
        {
            if (contentProvider == null) { throw new ArgumentNullException(nameof(contentProvider)); }

            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
            streamResolver = new EdgeUriToStreamResolver(contentProvider);

            Layout += (s, e) => UpdateSize();

            Init();
        }

        public void NavigateToFile(string url)
        {
            if (string.IsNullOrWhiteSpace(config.ExternalHost))
            {
                var uri = webview.BuildLocalStreamUri("spidereye", url);
                webview.NavigateToLocalStreamUri(uri, streamResolver);
            }
            else
            {
                var uri = UriTools.Combine(config.ExternalHost, url);
                webview.Navigate(uri);
            }
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await webview.InvokeScriptAsync("eval", new string[] { script });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                webview?.Close();
                webview = null;
            }
        }

        protected override void DestroyHandle()
        {
            Close();
            base.DestroyHandle();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Close();
            base.OnHandleDestroyed(e);
        }

        private void Close()
        {
            webview?.Close();
        }

        private void Init()
        {
            var process = new WebViewControlProcess();
            var bounds = new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

            webview = process.CreateWebViewControlAsync(Handle.ToInt64(), bounds)
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            UpdateSize();

            webview.DefaultBackgroundColor = ParseColor(config.BackgroundColor);
            webview.Settings.IsScriptNotifyAllowed = config.EnableScriptInterface;
            if (config.EnableScriptInterface)
            {
                webview.ScriptNotify += Webview_ScriptNotify;

                // TODO: needs Win10 1809 - 10.0.17763.0
                // webview.AddInitializeScript(initScript);
            }

            webview.NavigationCompleted += Webview_NavigationCompleted;
        }

        private async void Webview_ScriptNotify(IWebViewControl sender, WebViewControlScriptNotifyEventArgs e)
        {
            await bridge.HandleScriptCall(e.Value);
        }

        private async void Webview_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
        {
            if (e.IsSuccess && config.EnableScriptInterface)
            {
                string initScript = Resources.GetInitScript("Windows");
                await ExecuteScriptAsync(initScript);
            }

            PageLoaded?.Invoke(this, PageLoadEventArgs.GetFor(e.IsSuccess));
        }

        private void UpdateSize()
        {
            if (webview != null)
            {
                var rect = new Rect(
                    (float)ClientRectangle.X,
                    (float)ClientRectangle.Y,
                    (float)ClientRectangle.Width,
                    (float)ClientRectangle.Height);

                webview.Bounds = rect;
            }
        }

        private Color ParseColor(string hex)
        {
            ColorTools.ParseHex(hex, out byte r, out byte g, out byte b);

            return new Color { A = byte.MaxValue, R = r, G = g, B = b, };
        }
    }
}

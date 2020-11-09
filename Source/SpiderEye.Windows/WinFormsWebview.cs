using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiderEye.Bridge;
using SpiderEye.Windows.Interop;
using Windows.Foundation;
using Windows.UI;
using Windows.Web.UI;
using Windows.Web.UI.Interop;

namespace SpiderEye.Windows
{
    internal class WinFormsWebview : Control, IWebview, IWinFormsWebview
    {
        public event NavigatingEventHandler Navigating;
        public event PageLoadEventHandler PageLoaded;


        public Control Control
        {
            get { return this; }
        }

        public bool EnableScriptInterface
        {
            get { return webview.Settings.IsScriptNotifyAllowed; }
            set { webview.Settings.IsScriptNotifyAllowed = value; }
        }

        private readonly WebviewBridge bridge;
        private readonly bool supportsInitializeScript;
        private WebViewControl webview;

        public WinFormsWebview(WebviewBridge bridge)
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            var version = WinNative.GetOsVersion();
            supportsInitializeScript = version.MajorVersion >= 10 && version.BuildNumber >= 17763;

            var process = new WebViewControlProcess();
            var bounds = new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);

            webview = process.CreateWebViewControlAsync(Handle.ToInt64(), bounds)
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            UpdateSize();

            if (supportsInitializeScript)
            {
                string initScript = Resources.GetInitScript("Windows");
                webview.AddInitializeScript(initScript);
            }

            webview.ScriptNotify += Webview_ScriptNotify;

            webview.NavigationCompleted += Webview_NavigationCompleted;
            webview.NavigationStarting += Webview_NavigationStarting;
            Layout += (s, e) => UpdateSize();
        }

        public void UpdateBackgroundColor(byte r, byte g, byte b)
        {
            webview.DefaultBackgroundColor = new Color { A = byte.MaxValue, R = r, G = g, B = b, };
        }

        public void LoadUri(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            if (uri.IsAbsoluteUri) { webview.Navigate(uri); }
            else
            {
                var localUri = webview.BuildLocalStreamUri("spidereye", uri.ToString());
                webview.NavigateToLocalStreamUri(localUri, EdgeUriToStreamResolver.Instance);
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

        private async void Webview_ScriptNotify(IWebViewControl sender, WebViewControlScriptNotifyEventArgs e)
        {
            await bridge.HandleScriptCall(e.Value);
        }

        private void Webview_NavigationStarting(IWebViewControl sender, WebViewControlNavigationStartingEventArgs e)
        {
            var args = new NavigatingEventArgs(e.Uri);
            Navigating?.Invoke(this, args);
            e.Cancel = args.Cancel;
        }

        private async void Webview_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
        {
            if (e.IsSuccess && !supportsInitializeScript)
            {
                string initScript = Resources.GetInitScript("Windows");
                await ExecuteScriptAsync(initScript);
            }

            PageLoaded?.Invoke(this, new PageLoadEventArgs(e.Uri, e.IsSuccess));
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
    }
}

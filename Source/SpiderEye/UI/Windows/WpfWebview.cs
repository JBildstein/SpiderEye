using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Threading;
using global::Windows.Web.UI;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.Tools;
using SpiderEye.UI.Windows.Interop;
using Windows.Web.UI.Interop;
using Color = Windows.UI.Color;
using Rect = Windows.Foundation.Rect;
using Size = System.Windows.Size;

namespace SpiderEye.UI.Windows
{
    internal class WpfWebview : HwndHost, IWebview, IWpfWebview
    {
        public event PageLoadEventHandler PageLoaded;

        public object Control
        {
            get { return this; }
        }

        private readonly AppConfiguration config;
        private readonly WebviewBridge bridge;
        private readonly EdgeUriToStreamResolver streamResolver;

        private IntPtr window;
        private WebViewControl webview;

        public WpfWebview(AppConfiguration config, IntPtr parentWindow, IContentProvider contentProvider, WebviewBridge bridge)
        {
            if (contentProvider == null) { throw new ArgumentNullException(nameof(contentProvider)); }

            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
            streamResolver = new EdgeUriToStreamResolver(contentProvider);

            SizeChanged += (s, e) => UpdateSize(e.NewSize);

            Init(parentWindow);
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

        public string ExecuteScript(string script)
        {
            return RunSynchronous(ExecuteScriptAsync(script));
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

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            return new HandleRef(null, window);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Native.DestroyWindow(hwnd.Handle);
            Native.CheckLastError();
        }

        private void Init(IntPtr parentWindow)
        {
            Native.EnableMouseInPointer(true);
            Native.CheckLastError();

            window = Native.CreateBrowserWindow(parentWindow);

            var process = new WebViewControlProcess();
            var bounds = new Rect(0, 0, RenderSize.Width, RenderSize.Height);

            webview = RunSynchronous(process.CreateWebViewControlAsync(window.ToInt64(), bounds).AsTask());

            UpdateSize(RenderSize);

            webview.DefaultBackgroundColor = ParseColor(config.Window.BackgroundColor);
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

        private void Webview_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
        {
            if (e.IsSuccess && config.EnableScriptInterface)
            {
                string initScript = SpiderEye.Resources.GetInitScript("Windows");
                ExecuteScript(initScript);
            }

            PageLoaded?.Invoke(this, PageLoadEventArgs.GetFor(e.IsSuccess));
        }

        private void UpdateSize(Size size)
        {
            if (webview != null)
            {
                var rect = new Rect(
                    (float)VisualOffset.X,
                    (float)VisualOffset.Y,
                    (float)size.Width,
                    (float)size.Height);

                webview.Bounds = rect;
            }
        }

        private Color ParseColor(string hex)
        {
            ColorTools.ParseHex(hex, out byte r, out byte g, out byte b);

            return new Color { A = byte.MaxValue, R = r, G = g, B = b, };
        }

        private T RunSynchronous<T>(Task<T> task)
        {
            T result = default;
            Exception ex = null;
            var frame = new DispatcherFrame(true);

            task.ContinueWith(t =>
            {
                if (t.IsFaulted) { ex = t.Exception; }
                else { result = t.Result; }

                frame.Continue = false;
            });

            Dispatcher.PushFrame(frame);

            if (ex != null) { throw ex; }

            return result;
        }
    }
}

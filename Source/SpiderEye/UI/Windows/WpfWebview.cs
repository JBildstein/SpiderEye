using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using global::Windows.Web.UI;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.Scripting;
using SpiderEye.Tools;
using SpiderEye.UI.Windows.Interop;
using Windows.Web.UI.Interop;
using Color = Windows.UI.Color;
using Rect = Windows.Foundation.Rect;
using Size = System.Windows.Size;

namespace SpiderEye.UI.Windows
{
    internal class WpfWebview : FrameworkElement, IWebview, IWpfWebview
    {
        public event EventHandler WebviewLoaded;

        public ScriptHandler ScriptHandler { get; }

        public object Control
        {
            get { return this; }
        }

        private readonly AppConfiguration config;
        private readonly EdgeUriToStreamResolver streamResolver;

        private WebViewControl webview;

        public WpfWebview(IntPtr window, IContentProvider contentProvider, AppConfiguration config)
        {
            if (contentProvider == null) { throw new ArgumentNullException(nameof(contentProvider)); }

            this.config = config ?? throw new ArgumentNullException(nameof(config));

            streamResolver = new EdgeUriToStreamResolver(contentProvider);

            SizeChanged += (s, e) => UpdateSize(e.NewSize);

            if (config.EnableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);
            }

            Init(window);
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
            return ExecuteScriptAsync(script).GetAwaiter().GetResult();
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            return await webview.InvokeScriptAsync("eval", new string[] { script });
        }

        public void Dispose()
        {
            webview?.Close();
            webview = null;
        }

        private void Init(IntPtr window)
        {
            Native.EnableMouseInPointer(true);
            Native.CheckLastError();

            Dispatcher.InvokeAsync(
                async () =>
                {
                    var process = new WebViewControlProcess();
                    var bounds = new Rect(0, 0, RenderSize.Width, RenderSize.Height);

                    webview = await process.CreateWebViewControlAsync(window.ToInt64(), bounds);
                    UpdateSize(RenderSize);

                    webview.DefaultBackgroundColor = ParseColor(config.Window.BackgroundColor);
                    webview.Settings.IsScriptNotifyAllowed = config.EnableScriptInterface;
                    if (config.EnableScriptInterface)
                    {
                        webview.ScriptNotify += Webview_ScriptNotify;

                        // TODO: don't think script injection works yet
                        string initScript = SpiderEye.Resources.GetInitScript("Windows");
                        webview.NavigationCompleted += (s, e) => ExecuteScript(initScript);

                        // TODO: needs Win10 1809 - 10.0.17763.0
                        // webview.AddInitializeScript(initScript);
                    }

                    Dispatcher.Invoke(() => WebviewLoaded?.Invoke(this, EventArgs.Empty));
                },
                DispatcherPriority.Send);
        }

        private void Webview_ScriptNotify(IWebViewControl sender, WebViewControlScriptNotifyEventArgs e)
        {
            ScriptHandler.HandleScriptCall(e.Value);
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
            hex = hex?.TrimStart('#');
            if (string.IsNullOrWhiteSpace(hex) || hex.Length != 6)
            {
                hex = "FFFFFF";
            }

            return new Color
            {
                A = byte.MaxValue,
                R = Convert.ToByte(hex.Substring(0, 2), 16),
                G = Convert.ToByte(hex.Substring(2, 2), 16),
                B = Convert.ToByte(hex.Substring(4, 2), 16),
            };
        }
    }
}

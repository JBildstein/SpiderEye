using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using global::Windows.Web.UI;
using SpiderEye.Configuration;
using SpiderEye.Scripting;
using Windows.Web.UI.Interop;
using Color = Windows.UI.Color;
using Rect = Windows.Foundation.Rect;

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

        private readonly WindowConfiguration config;

        private HandleRef parentWindow;
        private HandleRef childWindow;
        private WebViewControl webview;

        public WpfWebview(IntPtr window, WindowConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            SizeChanged += (s, e) => UpdateSize(e.NewSize);

            if (config.EnableScriptInterface)
            {
                ScriptHandler = new ScriptHandler(this);
            }

            Init(window);
        }

        public void LoadUrl(string url)
        {
            webview.Source = new Uri(url);
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

        private void Init(IntPtr parentWindow)
        {
            Dispatcher.InvokeAsync(
                async () =>
                {
                    var process = new WebViewControlProcess();
                    var bounds = new Rect(0, 0, RenderSize.Width, RenderSize.Height);

                    webview = await process.CreateWebViewControlAsync(parentWindow.ToInt64(), bounds);
                    UpdateSize(RenderSize);

                    webview.DefaultBackgroundColor = ParseColor(config.BackgroundColor);
                    webview.Settings.IsScriptNotifyAllowed = config.EnableScriptInterface;
                    if (config.EnableScriptInterface)
                    {
                        webview.ScriptNotify += Webview_ScriptNotify;

                        // TODO: needs Win10 1809 - 10.0.17763.0
                        // webview.AddInitializeScript(SpiderEye.Resources.GetInitScript("Windows"));
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
            if (string.IsNullOrWhiteSpace(hex) || hex.Length != 6 || hex.Length != 3)
            {
                hex = "FFF";
            }

            string r, g, b;
            if (hex.Length == 3)
            {
                r = hex[0].ToString();
                g = hex[1].ToString();
                b = hex[2].ToString();
            }
            else
            {
                r = hex.Substring(0, 2);
                g = hex.Substring(2, 2);
                b = hex.Substring(4, 2);
            }

            return new Color
            {
                A = byte.MaxValue,
                R = Convert.ToByte(r, 16),
                G = Convert.ToByte(r, 16),
                B = Convert.ToByte(r, 16),
            };
        }
    }
}

// uncomment this to enable legacy webview even if it's supported:
// #define USE_LEGACY_WEBVIEW

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SpiderEye.Configuration;
using SpiderEye.UI.Windows.Interop;

namespace SpiderEye.UI.Windows
{
    internal class WpfWindow : Window, IWindow
    {
        public event EventHandler WindowReady;

        int IWindow.Width
        {
            get { return (int)Width; }
        }

        int IWindow.Height
        {
            get { return (int)Height; }
        }

        public string BackgroundColor { get; }

        public bool CanResize
        {
            get { return ResizeMode == ResizeMode.CanResize; }
            set { ResizeMode = value ? ResizeMode.CanResize : ResizeMode.NoResize; }
        }

        private readonly WindowConfiguration config;
        private IWpfWebview webview;

        public WpfWindow(WindowConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            if (UseLegacy())
            {
                webview = new WpfLegacyWebview(config.EnableScriptInterface);
                Loaded += (s, e) => WindowReady(this, EventArgs.Empty);
            }
            else
            {
                var helper = new WindowInteropHelper(this);
                var view = new WpfWebview(helper.EnsureHandle(), config);
                webview = view;
                view.WebviewLoaded += (s, e) => WindowReady(this, EventArgs.Empty);
            }

            AddChild(webview.Control);

            Title = config.Title;
            Width = config.Width;
            Height = config.Height;
            CanResize = config.CanResize;

            BackgroundColor = config.BackgroundColor;
            if (string.IsNullOrWhiteSpace(BackgroundColor)) { BackgroundColor = "#FFFFFF"; }
            Background = new BrushConverter().ConvertFrom(BackgroundColor) as SolidColorBrush;

            if (config.UseBrowserTitle && config.EnableScriptInterface)
            {
                webview.ScriptHandler.TitleChanged += (s, e) => Title = e ?? config.Title;
            }
        }

        public void Dispose()
        {
            webview.Dispose();
        }

        public void LoadUrl(string url)
        {
            webview.LoadUrl(url);
        }

        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case UI.WindowState.Normal:
                    WindowState = System.Windows.WindowState.Normal;
                    break;

                case UI.WindowState.Maximized:
                    WindowState = System.Windows.WindowState.Maximized;
                    break;

                case UI.WindowState.Minimized:
                    WindowState = System.Windows.WindowState.Minimized;
                    break;

                default:
                    throw new ArgumentException($"Invalid window state of \"{state}\"", nameof(state));
            }
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        private bool UseLegacy()
        {
#if USE_LEGACY_WEBVIEW
            return true;
#warning Legacy Webview is enabled!
#else
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll");
            if (!System.IO.File.Exists(path)) { return false; }

            var version = Native.GetOsVersion();
            return !(version.MajorVersion >= 10 && version.BuildNumber >= 17134);
#endif
        }
    }
}

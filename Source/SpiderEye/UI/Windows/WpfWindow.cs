using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.UI.Windows.Interop;

namespace SpiderEye.UI.Windows
{
    internal class WpfWindow : Window, IWindow
    {
        public event PageLoadEventHandler PageLoaded
        {
            add { webview.PageLoaded += value; }
            remove { webview.PageLoaded -= value; }
        }

        event CancelableEventHandler IWindow.Closing
        {
            add { ClosingBackingEvent += value; }
            remove { ClosingBackingEvent -= value; }
        }

        event EventHandler IWindow.Closed
        {
            add { ClosedBackingEvent += value; }
            remove { ClosedBackingEvent -= value; }
        }

        private event CancelableEventHandler ClosingBackingEvent;
        private event EventHandler ClosedBackingEvent;

        public bool CanResize
        {
            get { return ResizeMode == ResizeMode.CanResize; }
            set { ResizeMode = value ? ResizeMode.CanResize : ResizeMode.NoResize; }
        }

        public IWebviewBridge Bridge
        {
            get { return bridge; }
        }

        private readonly ContentServer server;
        private readonly WebviewBridge bridge;
        private IWpfWebview webview;

        public WpfWindow(AppConfiguration config, IWindowFactory windowFactory)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (windowFactory == null) { throw new ArgumentNullException(nameof(windowFactory)); }

            bridge = new WebviewBridge();
            var contentProvider = new EmbeddedFileProvider(config.ContentAssembly, config.ContentFolder);
            if (config.ForceWindowsLegacyWebview || UseLegacy())
            {
                string hostAddress;
                if (!string.IsNullOrWhiteSpace(config.ExternalHost))
                {
                    server = new ContentServer(contentProvider);
                    server.Start();
                    hostAddress = server.HostAddress;
                }
                else { hostAddress = config.ExternalHost; }

                webview = new WpfLegacyWebview(config, hostAddress, bridge);
            }
            else
            {
                var helper = new WindowInteropHelper(this);
                var view = new WpfWebview(config, helper.EnsureHandle(), contentProvider, bridge);
                webview = view;
            }

            AddChild(webview.Control);

            Title = config.Window.Title;
            Width = config.Window.Width;
            Height = config.Window.Height;
            CanResize = config.Window.CanResize;

            string backgroundColor = config.Window.BackgroundColor;
            if (string.IsNullOrWhiteSpace(backgroundColor)) { backgroundColor = "#FFFFFF"; }
            Background = new BrushConverter().ConvertFrom(backgroundColor) as SolidColorBrush;

            if (config.EnableScriptInterface) { bridge.Init(this, webview, windowFactory); }

            if (config.Window.UseBrowserTitle)
            {
                bridge.TitleChanged += (s, e) => Title = e ?? config.Window.Title;
            }

            SetIcon(config.Window.Icon);
        }

        public void Dispose()
        {
            webview.Dispose();
            server?.Dispose();
        }

        public void LoadUrl(string url)
        {
            webview.NavigateToFile(url);
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

        public void SetIcon(WindowIcon icon)
        {
            if (icon == null || icon.Icons.Count == 0) { Icon = null; }
            else
            {
                using (var stream = new MemoryStream(icon.Icons[0]))
                {
                    Icon = BitmapFrame.Create(stream);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var args = new CancelableEventArgs();
            ClosingBackingEvent?.Invoke(this, args);
            e.Cancel = args.Cancel;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            ClosedBackingEvent?.Invoke(this, EventArgs.Empty);
            base.OnClosed(e);
        }

        private bool UseLegacy()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "edgehtml.dll");
            if (!File.Exists(path)) { return false; }

            var version = Native.GetOsVersion();
            return !(version.MajorVersion >= 10 && version.BuildNumber >= 17134);
        }
    }
}

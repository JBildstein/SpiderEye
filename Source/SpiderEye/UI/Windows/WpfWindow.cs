using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SpiderEye.Configuration;
using SpiderEye.Content;
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

        private readonly ContentServer server;
        private IWpfWebview webview;

        public WpfWindow(AppConfiguration config)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

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

                webview = new WpfLegacyWebview(config, hostAddress);
                Loaded += (s, e) => WindowReady(this, EventArgs.Empty);
            }
            else
            {
                var helper = new WindowInteropHelper(this);
                var view = new WpfWebview(helper.EnsureHandle(), contentProvider, config);
                webview = view;
                view.WebviewLoaded += (s, e) => WindowReady(this, EventArgs.Empty);
            }

            AddChild(webview.Control);

            Title = config.Window.Title;
            Width = config.Window.Width;
            Height = config.Window.Height;
            CanResize = config.Window.CanResize;

            BackgroundColor = config.Window.BackgroundColor;
            if (string.IsNullOrWhiteSpace(BackgroundColor)) { BackgroundColor = "#FFFFFF"; }
            Background = new BrushConverter().ConvertFrom(BackgroundColor) as SolidColorBrush;

            if (config.Window.UseBrowserTitle && config.EnableScriptInterface)
            {
                webview.ScriptHandler.TitleChanged += (s, e) => Title = e ?? config.Window.Title;
            }
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

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
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

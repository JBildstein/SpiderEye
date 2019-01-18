using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Toolkit.Wpf.UI.Controls;
using SpiderEye.Configuration;

namespace SpiderEye.UI.Windows
{
    internal class WpfWindow : Window, IWindow
    {
        public IWebview Webview
        {
            get { return webview; }
        }

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
        private readonly IWpfWebview webview;

        public WpfWindow(WindowConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            // WebViewCompatible does not expose methods required for SpiderEye.
            // As a workaround, do the same thing as WebViewCompatible with the methods that are needed.
            if (WebViewCompatible.IsLegacy) { webview = new WpfLegacyWebview(config.EnableScriptInterface); }
            else { webview = new WpfWebview(config.EnableScriptInterface); }

            AddChild(webview.Control);

            Title = config.Title;
            Width = config.Width;
            Height = config.Height;
            CanResize = config.CanResize;

            BackgroundColor = config.BackgroundColor;
            if (string.IsNullOrWhiteSpace(BackgroundColor)) { BackgroundColor = "#FFFFFF"; }
            Background = new BrushConverter().ConvertFrom(BackgroundColor) as SolidColorBrush;

            if (config.UseBrowserTitle)
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
    }
}

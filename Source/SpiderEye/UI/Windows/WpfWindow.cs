using System;
using System.Windows;

namespace SpiderEye.UI.Windows
{
    internal class WpfWindow : Window, IWindow
    {
        public IWebview Webview
        {
            get { return webview; }
        }

        private readonly AppConfiguration config;
        private readonly IWpfWebview webview;

        public WpfWindow(AppConfiguration config, IWpfWebview webview)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.webview = webview ?? throw new ArgumentNullException(nameof(webview));

            AddChild(webview.Control);

            Title = config.Title;
            Width = config.Width;
            Height = config.Height;
            ResizeMode = config.CanResize ? ResizeMode.CanResize : ResizeMode.NoResize;

            webview.ScriptHandler.TitleChanged += (s, e) => Title = e ?? config.Title;
        }
    }
}

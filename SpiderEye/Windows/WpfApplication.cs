using System;
using Microsoft.Toolkit.Wpf.UI.Controls;

namespace SpiderEye.Windows
{
    public class WpfApplication : IApplication
    {
        public IWindow MainWindow
        {
            get { return window; }
        }

        private readonly AppConfiguration config;
        private readonly System.Windows.Application application;
        private readonly WpfWindow window;

        public WpfApplication(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            // WebViewCompatible does not expose InvokeScript(string, string[])
            // as a workaround, do the same as WebViewCompatible with only the methods that are needed
            IWpfWebview webview;
            if (WebViewCompatible.IsLegacy) { webview = new WpfLegacyWebview(config); }
            else { webview = new WpfWebview(config); }

            window = new WpfWindow(config, webview);

            application = new System.Windows.Application();
            application.MainWindow = window;
        }

        public void Run()
        {
            window.Webview.LoadUrl(config.Url);
            window.Show();

            application.Run();
        }
    }
}

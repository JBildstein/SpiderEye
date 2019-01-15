using Microsoft.Toolkit.Wpf.UI.Controls;

namespace SpiderEye.UI.Windows
{
    internal class WpfApplication : ApplicationBase
    {
        public override IWindow MainWindow
        {
            get { return window; }
        }

        public override IWebview Webview
        {
            get { return webview; }
        }

        private readonly System.Windows.Application application;
        private readonly WpfWindow window;
        private readonly IWpfWebview webview;

        public WpfApplication(AppConfiguration config)
            : base(config)
        {
            // WebViewCompatible does not expose InvokeScript(string, string[])
            // as a workaround, do the same as WebViewCompatible with only the methods that are needed
            if (WebViewCompatible.IsLegacy) { webview = new WpfLegacyWebview(config); }
            else { webview = new WpfWebview(config); }

            window = new WpfWindow(config, webview);

            application = new System.Windows.Application();
            application.MainWindow = window;
        }

        public override void Run()
        {
            base.Run();
            application.Run();

            webview.Control.Dispose();
        }

        public override void Exit()
        {
            application.Shutdown();
        }
    }
}

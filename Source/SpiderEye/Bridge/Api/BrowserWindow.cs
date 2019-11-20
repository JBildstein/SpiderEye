using System;
using SpiderEye.Bridge.Models;
using SpiderEye.UI;

namespace SpiderEye.Bridge.Api
{
    internal class BrowserWindow
    {
        private readonly IWindow parent;
        private readonly IUiFactory windowFactory;

        public BrowserWindow(IWindow parent, IUiFactory windowFactory)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));
        }

        public void Show(BrowserWindowConfigModel config)
        {
            // TODO: BrowserWindowConfigModel cannot hold all possible information (like Icon and Assembly)
            // TODO: transfer some configs from parent window (e.g. ExternalHost)
            Application.Invoke(() =>
            {
                var window = windowFactory.CreateWindow(config.WindowConfig);
                window.LoadUrl(config.Url);
                window.Show();
            });

            // TODO: somehow send events from this window back to webview
        }
    }
}

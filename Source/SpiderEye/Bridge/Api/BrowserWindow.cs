using System;
using SpiderEye.Bridge.Models;
using SpiderEye.UI;

namespace SpiderEye.Bridge.Api
{
    internal class BrowserWindow
    {
        private readonly IWindow parent;
        private readonly IWindowFactory windowFactory;

        public BrowserWindow(IWindow parent, IWindowFactory windowFactory)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.windowFactory = windowFactory ?? throw new ArgumentNullException(nameof(windowFactory));
        }

        public void Show(BrowserWindowConfigModel config)
        {
            var window = windowFactory.CreateWindow(config);
            window.LoadUrl(config.Url);
            window.Show();

            // TODO: somehow send events from this window back to webview
        }
    }
}

using System;
using System.Collections.Generic;
using SpiderEye.Bridge.Models;
using SpiderEye.UI;

namespace SpiderEye.Bridge.Api
{
    internal class BrowserWindow
    {
        private readonly IWindow parent;
        private readonly IWindowFactory windowFactory;

        private readonly List<IWindow> windows = new List<IWindow>();

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
            windows.Add(window);

            // TODO: somehow send events from this window back to webview
        }
    }
}

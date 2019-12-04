using System;
using System.Collections.Generic;
using SpiderEye.Bridge.Models;

namespace SpiderEye.Bridge.Api
{
    [BridgeObject("f0631cfea99a_Window")]
    internal class WindowApiBridge
    {
        private static readonly WindowCollection WindowStore = new WindowCollection();
        private readonly Window parent;

        public WindowApiBridge(Window parent)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public void Show(BrowserWindowConfigModel config)
        {
            Application.Invoke(() =>
            {
                var window = new Window
                {
                    Title = config.Title ?? Window.DefaultConfig.Title,
                    Size = new Size(
                        config.Width ?? Window.DefaultConfig.Size.Width,
                        config.Height ?? Window.DefaultConfig.Size.Height),
                    MinSize = new Size(
                        config.MinWidth ?? Window.DefaultConfig.MinSize.Width,
                        config.MinHeight ?? Window.DefaultConfig.MinSize.Height),
                    MaxSize = new Size(
                        config.MaxWidth ?? Window.DefaultConfig.MaxSize.Width,
                        config.MaxHeight ?? Window.DefaultConfig.MaxSize.Height),
                    BackgroundColor = config.BackgroundColor ?? Window.DefaultConfig.BackgroundColor,
                    CanResize = config.CanResize ?? Window.DefaultConfig.CanResize,
                    UseBrowserTitle = config.UseBrowserTitle ?? Window.DefaultConfig.UseBrowserTitle,
                    EnableScriptInterface = config.EnableScriptInterface ?? Window.DefaultConfig.EnableScriptInterface,
                    EnableDevTools = config.EnableDevTools ?? Window.DefaultConfig.EnableDevTools,
                    Icon = parent.Icon,
                };

                WindowStore.Add(window);

                window.LoadUrl(config.Url);
                window.Show();
            });

            // TODO: somehow send events from this window back to webview
        }

        private sealed class WindowCollection
        {
            private readonly List<Window> windows = new List<Window>();

            public void Add(Window window)
            {
                windows.Add(window);

                // both this method and the Closed event are run on the main thread,
                // so it's save to access the windows list without a lock
                window.Closed += (s, e) => windows.Remove(window);
            }
        }
    }
}

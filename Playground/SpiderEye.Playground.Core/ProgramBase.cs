using System;
using System.Diagnostics;

namespace SpiderEye.Playground.Core
{
    public abstract class ProgramBase
    {
        protected static void Run()
        {
            var icon = AppIcon.FromFile("icon", ".");

            using var statusIcon = new StatusIcon();
            using var window = new Window();
            window.Title = "SpiderEye Playground";
            window.UseBrowserTitle = true;
            window.EnableScriptInterface = true;
            window.CanResize = true;
            window.BackgroundColor = "#303030";
            window.Size = new Size(800, 600);
            window.MinSize = new Size(300, 200);
            window.MaxSize = new Size(1200, 900);
            window.Icon = icon;

            statusIcon.Icon = icon;
            statusIcon.Title = window.Title;

            SetDevSettings(window);

            var menu = new Menu();
            var showItem = menu.MenuItems.AddLabelItem("Hello World");
            showItem.SetShortcut(ModifierKey.Primary, Key.O);
            showItem.Click += ShowItem_Click;

            var eventItem = menu.MenuItems.AddLabelItem("Send Event to Webview");
            eventItem.SetShortcut(ModifierKey.Primary, Key.E);
            eventItem.Click += async (s, e) => await window.Bridge.InvokeAsync("dateUpdated", DateTime.Now);

            var subMenuItem = menu.MenuItems.AddLabelItem("Open me!");
            subMenuItem.MenuItems.AddLabelItem("Boo!");

            var borderItem = menu.MenuItems.AddLabelItem("Window Border");
            var def = borderItem.MenuItems.AddLabelItem("Default");
            def.Click += (s, e) => { window.BorderStyle = WindowBorderStyle.Default; };
            var none = borderItem.MenuItems.AddLabelItem("None");
            none.Click += (s, e) => { window.BorderStyle = WindowBorderStyle.None; };

            var sizeItem = menu.MenuItems.AddLabelItem("Window Size");
            var max = sizeItem.MenuItems.AddLabelItem("Maximize");
            max.Click += (s, e) => { window.Maximize(); };
            var unmax = sizeItem.MenuItems.AddLabelItem("Unmaximize");
            unmax.Click += (s, e) => { window.Unmaximize(); };
            var min = sizeItem.MenuItems.AddLabelItem("Minimize");
            min.Click += (s, e) => { window.Minimize(); };
            var unmin = sizeItem.MenuItems.AddLabelItem("Unminimize");
            unmin.Click += (s, e) => { window.Unminimize(); };
            var full = sizeItem.MenuItems.AddLabelItem("Enter Fullscreen");
            full.Click += (s, e) => { window.EnterFullscreen(); };
            var unfull = sizeItem.MenuItems.AddLabelItem("Exit Fullscreen");
            unfull.SetShortcut(ModifierKey.Primary, Key.F11);
            unfull.Click += (s, e) => { window.ExitFullscreen(); };

            menu.MenuItems.AddSeparatorItem();

            var exitItem = menu.MenuItems.AddLabelItem("Exit");
            exitItem.Click += (s, e) => Application.Exit();

            statusIcon.Menu = menu;

            var bridge = new UiBridge();
            Application.AddGlobalHandler(bridge);

            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            Application.UriWatcher = new AngularDevUriWatcher("http://localhost:65400");
            Application.ContentProvider = new EmbeddedContentProvider("Angular/dist");

            Application.Run(window, "/index.html");
        }

        private static void ShowItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                $"Hello World from the SpiderEye Playground running on {Application.OS}",
                "Hello World",
                MessageBoxButtons.Ok);
        }

        [Conditional("DEBUG")]
        private static void SetDevSettings(Window window)
        {
            window.EnableDevTools = true;
            window.PageLoaded += (s, e) =>
            {
                if (!e.Success)
                {
                    string message = $"Page did not load!{Environment.NewLine}Did you start the Angular dev server?";
                    if (Application.OS == OperatingSystem.Windows)
                    {
                        message += $"{Environment.NewLine}On Windows 10 you also have to allow localhost. More info can be found in the SpiderEye readme.";
                    }

                    MessageBox.Show(window, message, "Page load failed", MessageBoxButtons.Ok);
                }
            };
        }
    }
}

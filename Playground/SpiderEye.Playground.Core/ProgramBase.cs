using System;
using System.Diagnostics;

namespace SpiderEye.Playground.Core
{
    public abstract class ProgramBase
    {
        private static Window _mainWindow;

        protected static void Run()
        {
            var icon = AppIcon.FromFile("icon", ".");

            using (var statusIcon = new StatusIcon())
            using (var window = new Window())
            {
                _mainWindow = window;
                window.Title = "SpiderEye Playground";
                window.UseBrowserTitle = true;
                window.EnableScriptInterface = true;
                window.CanResize = true;
                window.BackgroundColor = "#303030";
                window.Size = new Size(800, 600);
                window.MinSize = new Size(300, 200);
                window.MaxSize = new Size(1200, 900);
                window.Icon = icon;
                window.UriChanged += (sender, uri) => Console.WriteLine("uri changed: " + uri);

                var windowMenu = new Menu();
                var appMenu = windowMenu.MenuItems.AddLabelItem(string.Empty);
                var quitMenu = appMenu.MenuItems.AddLabelItem("Quit");
                quitMenu.SetSystemShortcut(SystemShortcut.Close);
                quitMenu.Click += (s, e) => Application.Exit();

                var mainMenu = windowMenu.MenuItems.AddLabelItem("Main Menu");
                mainMenu.MenuItems.AddLabelItem("Entry 1");
                mainMenu.MenuItems.AddSeparatorItem();
                mainMenu.MenuItems.AddLabelItem("Entry 2");
                var showModalMenu = mainMenu.MenuItems.AddLabelItem("Show Modal");
                showModalMenu.Click += ShowModalMenu_Click;
                showModalMenu.SetShortcut(ModifierKey.Control | ModifierKey.Shift, Key.M);

                var helpMenu = windowMenu.MenuItems.AddLabelItem("Help");
                var helpItem = helpMenu.MenuItems.AddLabelItem("MyHelp");
                helpItem.SetSystemShortcut(SystemShortcut.Help);

                window.Menu = windowMenu;

                statusIcon.Icon = icon;
                statusIcon.Title = window.Title;

                if (window.MacOsOptions != null)
                {
                    window.MacOsOptions.Appearance = MacOsAppearance.DarkAqua;
                }

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
        }

        private static void ShowModalMenu_Click(object sender, EventArgs e)
        {
            var modal = new Window { Title = "this is a modal" };
            modal.Closed += DisposeWindow;
            _mainWindow.ShowModal(modal);
        }

        private static void DisposeWindow(object sender, EventArgs e)
        {
            if (!(sender is Window d))
            {
                return;
            }

            d.Closed -= DisposeWindow;
            d.Dispose();
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
                        message +=
                            $"{Environment.NewLine}On Windows 10 you also have to allow localhost. More info can be found in the SpiderEye readme.";
                    }

                    MessageBox.Show(window, message, "Page load failed", MessageBoxButtons.Ok);
                }
            };
        }
    }
}

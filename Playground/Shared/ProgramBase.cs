using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SpiderEye.UI;

namespace SpiderEye.Playground.Core
{
    public abstract class ProgramBase
    {
        protected static WindowConfiguration GetWindowConfiguration()
        {
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var config = new WindowConfiguration()
            {
                Title = "SpiderEye Playground",
                UseBrowserTitle = true,
                EnableScriptInterface = true,
                CanResize = true,
                BackgroundColor = "#303030",
                Width = 800,
                Height = 600,
                ContentFolder = "Angular\\dist",
                ContentAssembly = typeof(UiBridge).Assembly,
                Icon = AppIcon.FromFile("icon", exeDir),
            };

            SetDevServer(config);

            return config;
        }

        protected static void Run(WindowConfiguration config)
        {
            using var statusIcon = Application.CreateStatusIcon();

            statusIcon.Icon = config.Icon;
            statusIcon.Title = config.Title;

            using var window = Application.CreateWindow(config);

            var menu = statusIcon.AddMenu();
            var showItem = menu.AddLabelMenuItem("Hello World");
            showItem.Click += ShowItem_Click;
            var eventItem = menu.AddLabelMenuItem("Send Event to Webview");
            eventItem.Click += async (s, e) => await window.Bridge.InvokeAsync("dateUpdated", DateTime.Now);
            menu.AddSeparatorMenuItem();
            var exitItem = menu.AddLabelMenuItem("Exit");
            exitItem.Click += (s, e) => Application.Exit();

            var bridge = new UiBridge();
            Application.AddGlobalHandler(bridge);

            Application.Run(window, "index.html");
        }

        private static void ShowItem_Click(object sender, EventArgs e)
        {
            var msg = Application.Factory.CreateMessageBox();
            msg.Buttons = MessageBoxButtons.Ok;
            msg.Title = "Hello World";
            msg.Message = $"Hello World from the SpiderEye Playground running on {Application.OS}";
            msg.Show();
        }

        [Conditional("DEBUG")]
        private static void SetDevServer(WindowConfiguration config)
        {
            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            config.ExternalHost = "http://localhost:65400";
        }
    }
}

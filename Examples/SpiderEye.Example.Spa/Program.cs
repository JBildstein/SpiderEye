using System;
using System.Diagnostics;
using SpiderEye.UI;

namespace SpiderEye.Example.Spa
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // this creates a new configuration with default values
            var config = new WindowConfiguration();
            var icon = AppIcon.FromFile("icon", "Icons");

            // this relates to the path defined in the .csproj file
            config.ContentFolder = "Angular\\dist";
            config.Icon = icon;

            // this is only called in Debug mode:
            SetDevServer(config);

            // runs the application and opens a window with the given page loaded
            Application.Run(config, "index.html");
        }

        [Conditional("DEBUG")]
        private static void SetDevServer(WindowConfiguration config)
        {
            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            config.ExternalHost = "http://localhost:65000";
        }
    }
}

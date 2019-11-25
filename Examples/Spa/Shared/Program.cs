using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SpiderEye.UI;

namespace SpiderEye.Example.Spa
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Note: Program.cs is shared between all projects for convenience.
            // You could just as well have a separate startup logic for each platform.

            // this creates a new configuration with default values
            var config = new WindowConfiguration();

            // this gets the full directory path where the executable is and in extension, the icon file.
            // this is used because Directory.GetCurrentDirectory() doesn't always work as expected
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var icon = AppIcon.FromFile("icon", exeDir);

            // we have a separate assembly for the client side files
            config.ContentAssembly = typeof(AssemblyMarker).Assembly;

            // this relates to the path defined in the core .csproj file
            config.ContentFolder = "Angular\\dist";
            config.Icon = icon;

            using (var window = Application.CreateWindow(config))
            {
                // these are only called in Debug mode:
                SetDevSettings(config);
                AddPageLoadWarning(window);

                // runs the application and opens the window with the given page loaded
                Application.Run(window, "index.html");
            }
        }

        [Conditional("DEBUG")]
        private static void SetDevSettings(WindowConfiguration config)
        {
            config.EnableDevTools = true;

            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            config.ExternalHost = "http://localhost:65000";
        }

        [Conditional("DEBUG")]
        private static void AddPageLoadWarning(IWindow window)
        {
            // this is just to give some suggestions in case something isn't set up correctly for development
            window.PageLoaded += (s, e) =>
            {
                if (!e.Success)
                {
                    var msgBox = Application.Factory.CreateMessageBox();
                    msgBox.Title = "Page load failed";
                    string message = $"Page did not load!{Environment.NewLine}Did you start the Angular dev server?";
                    if (Application.OS == OperatingSystem.Windows)
                    {
                        message += $"{Environment.NewLine}On Windows 10 you also have to allow localhost. More info can be found in the SpiderEye readme.";
                    }

                    msgBox.Message = message;
                    msgBox.Buttons = MessageBoxButtons.Ok;
                    msgBox.Show(window);
                }
            };
        }
    }
}

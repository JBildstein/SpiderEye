using System;
using System.Diagnostics;

namespace SpiderEye.Example.Spa.Core
{
    public abstract class ProgramBase
    {
        protected static void Run()
        {
            // this creates a new window with default values
            using (var window = new Window())
            {
                window.Icon = AppIcon.FromFile("icon", ".");

                // these are only called in Debug mode:
                SetDevSettings(window);

                // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
                // note that you have to run the angular dev server first (npm run watch)
                Application.UriWatcher = new AngularDevUriWatcher("http://localhost:65400");

                // this relates to the path defined in the .csproj file
                Application.ContentProvider = new EmbeddedContentProvider("Angular\\dist");

                // runs the application and opens the window with the given page loaded
                Application.Run(window, "/index.html");
            }
        }

        [Conditional("DEBUG")]
        private static void SetDevSettings(Window window)
        {
            window.EnableDevTools = true;

            // this is just to give some suggestions in case something isn't set up correctly for development
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

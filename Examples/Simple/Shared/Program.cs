using System;
using SpiderEye.UI;

namespace SpiderEye.Example.Simple
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Note: Program.cs is shared between all projects for convenience.
            // You could just as well have a separate startup logic for each platform.

            // this creates a new app configuration with default values
            var config = new WindowConfiguration();

            // we have a separate assembly for the client side files
            config.ContentAssembly = typeof(AssemblyMarker).Assembly;

            // this relates to the path defined in the core .csproj file
            config.ContentFolder = "App";

            // runs the application and opens a window with the given page loaded
            Application.Run(config, "index.html");
        }
    }
}

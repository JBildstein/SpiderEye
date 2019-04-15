using System;
using SpiderEye.UI;

namespace SpiderEye.Example.Simple
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // this creates a new app configuration with default values
            var config = new WindowConfiguration();

            // this relates to the path defined in the .csproj file
            config.ContentFolder = "App";

            // runs the application and opens a window with the given page loaded
            Application.Run(config, "index.html");
        }
    }
}

using System;
using System.Diagnostics;
using SpiderEye.Configuration;

namespace SpiderEye.Example.Spa
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // this creates a new app configuration with default values
            var config = new AppConfiguration();

            // this relates to the path defined in the .csproj file
            config.ContentFolder = "Angular\\dist";

            // this is only called in Debug mode:
            SetDevServer(config);

            Application.Run(config);
        }

        [Conditional("DEBUG")]
        private static void SetDevServer(AppConfiguration config)
        {
            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            config.ExternalHost = "http://localhost:55000";
        }
    }
}

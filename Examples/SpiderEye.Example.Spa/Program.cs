using System;
using System.Diagnostics;
using SpiderEye.Configuration;
using SpiderEye.Example.Spa.Controllers;

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
            config.Server.ContentFolder = "Angular\\dist";

            // this is only called in Debug mode:
            SetDevServer(config);

            // register a controller so we can call it later from the webview
            config.Server.RegisterController<ExampleController>();

            Application.Run(config);
        }

        [Conditional("DEBUG")]
        private static void SetDevServer(AppConfiguration config)
        {
            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            config.Server.Host = "http://localhost:55000";

            // this is the port number for the SpiderEye server. Relates to the value(s) in the proxy.conig.json file.
            // for developing it needs a fixed value to proxy requests to the angular dev server to the internal server.
            config.Server.Port = 55100;
        }
    }
}

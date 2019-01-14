using System;
using System.Diagnostics;
using SpiderEye.Example.Spa.Controllers;

namespace SpiderEye.Example.Spa
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var config = new AppConfiguration
            {
                Title = "Hello World",
                Width = 900,
                Height = 600,
                CanResize = true,
                StartPageUrl = "index.html",
                ContentFolder = "Angular\\dist", // this relates to the path defined in the .csproj file
            };

            // this is only called in Debug mode:
            SetDevServer(config);

            ControllerRegistry.Register<ExampleController>();

            Application.Run(config);
        }

        [Conditional("DEBUG")]
        private static void SetDevServer(AppConfiguration config)
        {
            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first (npm run watch)
            config.Host = "http://localhost:55000";

            // this is the port number for the SpiderEye server. Relates to the value(s) in the proxy.conig.json file.
            // for developing it needs a fixed value to proxy requests to the angular dev server to the internal server.
            config.Port = 55100;
        }
    }
}

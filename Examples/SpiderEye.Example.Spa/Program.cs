using System;
using System.Diagnostics;
using SpiderEye.AspNetCore;

namespace SpiderEye
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

            SpiderEyeWebHost.Run(config, args);
        }

        [Conditional("DEBUG")]
        private static void SetDevServer(AppConfiguration config)
        {
            // the port number is defined in the angular.json file (under "architect"->"serve"->"options"->"port")
            // note that you have to run the angular dev server first
            config.Host = "http://localhost:55000";
        }
    }
}

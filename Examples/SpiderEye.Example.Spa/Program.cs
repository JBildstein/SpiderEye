using System;
using Microsoft.AspNetCore.Hosting;
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
                Url = "index.html",
                ContentFolder = "Angular\\dist",
                ShowDevTools = true,
            };

#if DEBUG
            string environment = EnvironmentName.Development;
            config.Host = "http://localhost:55000";
#else
            string environment = EnvironmentName.Production;
#endif

            SpiderEyeWebHost.Run(config, args, environment);
        }
    }
}

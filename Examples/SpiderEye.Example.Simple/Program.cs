using System;
using SpiderEye.Configuration;

namespace SpiderEye.Example.Simple
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // this creates a new app configuration with default values
            var config = new AppConfiguration();

            // this relates to the path defined in the .csproj file
            config.Server.ContentFolder = "App";

            Application.Run(config);
        }
    }
}

using System;

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
                ContentFolder = "App", // this relates to the path defined in the .csproj file
            };

            Application.Run(config);
        }
    }
}

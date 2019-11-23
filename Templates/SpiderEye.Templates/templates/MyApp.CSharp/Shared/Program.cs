using System;
using System.IO;
using System.Reflection;
using SpiderEye;
using SpiderEye.UI;

namespace MyApp
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var config = new WindowConfiguration()
            {
                Title = "MyApp",
                ContentFolder = "App",
                ContentAssembly = typeof(AssemblyMarker).Assembly,
                Icon = AppIcon.FromFile("icon", exeDir),
            };
            
            Application.Run(config, "index.html");
        }
    }
}

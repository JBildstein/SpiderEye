using System;
using SpiderEye.Mac;
using MyApp.Core;

namespace MyApp
{
    class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MacApplication.Init();
            Run();
        }
    }
}

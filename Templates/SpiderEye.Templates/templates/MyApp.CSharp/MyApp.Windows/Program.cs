using System;
using SpiderEye.Windows;
using MyApp.Core;

namespace MyApp
{
    class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            WindowsApplication.Init();
            Run();
        }
    }
}

using System;
using SpiderEye.Windows;
using SpiderEye.Example.Spa.Core;

namespace SpiderEye.Example.Spa
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

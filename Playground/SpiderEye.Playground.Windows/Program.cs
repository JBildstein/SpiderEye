using System;
using SpiderEye.Playground.Core;
using SpiderEye.Windows;

namespace SpiderEye.Playground
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

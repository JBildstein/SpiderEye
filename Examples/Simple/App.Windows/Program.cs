using System;
using SpiderEye.Windows;
using SpiderEye.Example.Simple.Core;

namespace SpiderEye.Example.Simple
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

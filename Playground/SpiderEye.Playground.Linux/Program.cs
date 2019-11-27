using System;
using SpiderEye.Linux;
using SpiderEye.Playground.Core;

namespace SpiderEye.Playground
{
    class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            LinuxApplication.Init();
            Run();
        }
    }
}

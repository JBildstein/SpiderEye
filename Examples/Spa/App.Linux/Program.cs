using System;
using SpiderEye.Linux;
using SpiderEye.Example.Spa.Core;

namespace SpiderEye.Example.Spa
{
    class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            LinuxApplication.Init();
            Run();
        }
    }
}

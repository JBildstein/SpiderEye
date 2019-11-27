using System;
using SpiderEye.Mac;
using SpiderEye.Playground.Core;

namespace SpiderEye.Playground
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

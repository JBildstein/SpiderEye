using System;
using SpiderEye.Mac;
using SpiderEye.Example.Spa.Core;

namespace SpiderEye.Example.Spa
{
    class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            MacApplication.Init();
            Run();
        }
    }
}

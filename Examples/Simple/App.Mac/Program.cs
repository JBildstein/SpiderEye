using System;
using SpiderEye.Mac;
using SpiderEye.Example.Simple.Core;

namespace SpiderEye.Example.Simple
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

using SpiderEye.Mac;
using SpiderEye.Playground.Core;

namespace SpiderEye.Playground
{
    class Program : ProgramBase
    {
        public static void Main()
        {
            MacApplication.Init();
            Run();
        }
    }
}

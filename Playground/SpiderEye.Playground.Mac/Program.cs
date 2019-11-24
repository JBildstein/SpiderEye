using System;
using SpiderEye.Playground.Core;

namespace SpiderEye.Playground
{
    class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var config = GetWindowConfiguration();

            using var appMenu = Application.CreateDefaultAppMenu();

            Run(config);
        }
    }
}

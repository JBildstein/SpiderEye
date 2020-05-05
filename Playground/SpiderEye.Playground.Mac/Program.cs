using SpiderEye.Mac;
using SpiderEye.Playground.Core;

namespace SpiderEye.Playground
{
    class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            MacApplication.Init();
            var menu = new Menu();
            var appMenu = menu.MenuItems.AddLabelItem(string.Empty);
            appMenu.MenuItems.AddAboutItem();
            appMenu.MenuItems.AddQuitItem();

            var helpMenu = menu.MenuItems.AddLabelItem("Help");
            helpMenu.MenuItems.AddHelpItem();

            MacApplication.AppMenu = menu;
            Run();
        }
    }
}

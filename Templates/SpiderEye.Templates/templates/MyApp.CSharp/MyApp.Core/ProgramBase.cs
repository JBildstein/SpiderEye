using SpiderEye;

namespace MyApp.Core
{
    public abstract class ProgramBase
    {
        protected static void Run()
        {
            using (var window = new Window())
            {
                window.Title = "MyApp";
                window.Icon = AppIcon.FromFile("icon", ".");

                Application.ContentProvider = new EmbeddedContentProvider("App");
                Application.Run(window, "index.html");
            }
        }
    }
}

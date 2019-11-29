namespace SpiderEye.Example.Simple.Core
{
    public abstract class ProgramBase
    {
        protected static void Run()
        {
            // this creates a new window with default values
            using (var window = new Window())
            {
                // this relates to the path defined in the .csproj file
                Application.ContentProvider = new EmbeddedContentProvider("App");

                // runs the application and opens the window with the given page loaded
                Application.Run(window, "index.html");
            }
        }
    }
}

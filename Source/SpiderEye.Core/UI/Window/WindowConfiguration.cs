using System.Reflection;

namespace SpiderEye
{
    internal class WindowConfiguration
    {
        public string? Title { get; }
        public Size Size { get; }
        public Size MinSize { get; }
        public Size MaxSize { get; }
        public string? BackgroundColor { get; }
        public bool CanResize { get; }
        public bool UseBrowserTitle { get; }
        public bool EnableScriptInterface { get; }
        public bool EnableDevTools { get; }

        public WindowConfiguration()
        {
            Title = Assembly.GetEntryAssembly()?.GetName().Name;
            Size = new Size(900, 600);
            MinSize = Size.Zero;
            MaxSize = Size.Zero;
            BackgroundColor = "#FFFFFF";
            CanResize = true;
            UseBrowserTitle = true;
            EnableScriptInterface = true;
            EnableDevTools = false;
        }
    }
}

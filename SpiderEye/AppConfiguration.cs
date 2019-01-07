namespace SpiderEye
{
    public class AppConfiguration
    {
        public static readonly AppConfiguration Default = new AppConfiguration();

        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool CanResize { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }
        public string ContentFolder { get; set; }
        public bool ShowDevTools { get; set; }

        public AppConfiguration()
        {
            Title = "Main Window";
            Width = 900;
            Height = 600;
            CanResize = true;
            Url = "index.html";
            ContentFolder = "App";
            ShowDevTools = false;
            Host = null;
        }
    }
}

namespace SpiderEye.Bridge.Models
{
    internal class BrowserWindowConfigModel
    {
        public string Title { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? MinWidth { get; set; }
        public int? MinHeight { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public string BackgroundColor { get; set; }
        public bool? CanResize { get; set; }
        public bool? UseBrowserTitle { get; set; }
        public bool? EnableScriptInterface { get; set; }
        public bool? EnableDevTools { get; set; }
        public string Url { get; set; }
    }
}

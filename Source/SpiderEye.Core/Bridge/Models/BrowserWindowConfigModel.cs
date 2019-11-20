using Newtonsoft.Json;
using SpiderEye.UI;

namespace SpiderEye.Bridge.Models
{
    internal class BrowserWindowConfigModel
    {
        public string Title
        {
            get { return WindowConfig.Title; }
            set { WindowConfig.Title = value; }
        }

        public int Width
        {
            get { return WindowConfig.Width; }
            set { WindowConfig.Width = value; }
        }

        public int Height
        {
            get { return WindowConfig.Height; }
            set { WindowConfig.Height = value; }
        }

        public string BackgroundColor
        {
            get { return WindowConfig.BackgroundColor; }
            set { WindowConfig.BackgroundColor = value; }
        }

        public bool CanResize
        {
            get { return WindowConfig.CanResize; }
            set { WindowConfig.CanResize = value; }
        }

        public bool UseBrowserTitle
        {
            get { return WindowConfig.UseBrowserTitle; }
            set { WindowConfig.UseBrowserTitle = value; }
        }

        public bool EnableScriptInterface
        {
            get { return WindowConfig.EnableScriptInterface; }
            set { WindowConfig.EnableScriptInterface = value; }
        }

        public string ExternalHost
        {
            get { return WindowConfig.ExternalHost; }
            set { WindowConfig.ExternalHost = value; }
        }

        public string ContentFolder
        {
            get { return WindowConfig.ContentFolder; }
            set { WindowConfig.ContentFolder = value; }
        }

        public bool ForceWindowsLegacyWebview
        {
            get { return WindowConfig.ForceWindowsLegacyWebview; }
            set { WindowConfig.ForceWindowsLegacyWebview = value; }
        }

        public string Url { get; set; }

        [JsonIgnore]
        public readonly WindowConfiguration WindowConfig = new WindowConfiguration();
    }
}

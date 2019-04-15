using System.Reflection;

namespace SpiderEye.UI
{
    /// <summary>
    /// Window configuration.
    /// </summary>
    public class WindowConfiguration
    {
        /// <summary>
        /// Gets or sets the initial window title. Default is "SpiderEye".
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the initial window width. Default is 900.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the initial window height. Default is 600.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the background color of the window. Default is "#FFFFFF".
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window can be resized or not. Default is true.
        /// </summary>
        public bool CanResize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window title should be updated when the browser title does. Default is true.
        /// </summary>
        public bool UseBrowserTitle { get; set; }

        /// <summary>
        /// Gets or sets the default window icon. Default is null.
        /// </summary>
        public AppIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the scripting interface between browser and window is enabled. Default is true.
        /// </summary>
        public bool EnableScriptInterface { get; set; }

        /// <summary>
        /// Gets or sets an external host. Default is null.
        /// </summary>
        public string ExternalHost { get; set; }

        /// <summary>
        /// Gets or sets the folder path where the embedded files are. Default is "App".
        /// </summary>
        public string ContentFolder { get; set; }

        /// <summary>
        /// Gets or sets the assembly where the content files are embedded. Default is the entry assembly.
        /// </summary>
        public Assembly ContentAssembly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force usage of the legacy
        /// webview on Windows even if the new one is supported. Default is false.
        /// </summary>
        public bool ForceWindowsLegacyWebview { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowConfiguration"/> class.
        /// </summary>
        public WindowConfiguration()
        {
            Title = "SpiderEye";
            Width = 900;
            Height = 600;
            BackgroundColor = "#FFFFFF";
            CanResize = true;
            UseBrowserTitle = true;
            Icon = null;
            EnableScriptInterface = true;
            ExternalHost = null;
            ContentFolder = "App";
            ContentAssembly = Assembly.GetEntryAssembly();
            ForceWindowsLegacyWebview = false;
        }
    }
}

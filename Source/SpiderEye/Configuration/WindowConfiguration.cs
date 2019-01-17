namespace SpiderEye.Configuration
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
        /// Gets or sets a value indicating whether the scripting interface between browser and window is enabled. Default is true.
        /// </summary>
        public bool EnableScriptInterface { get; set; }

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
            EnableScriptInterface = true;
        }
    }
}

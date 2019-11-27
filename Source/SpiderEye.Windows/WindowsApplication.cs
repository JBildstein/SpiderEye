namespace SpiderEye.Windows
{
    /// <summary>
    /// Provides Windows specific application methods.
    /// </summary>
    public static class WindowsApplication
    {
        /// <summary>
        /// Gets or sets which webview version should be used at most.
        /// </summary>
        public static WebviewType WebviewType { get; set; }

        private static WinFormsApplication app;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Init()
        {
            WebviewType = WebviewType.Latest;

            app = new WinFormsApplication();
            Application.Register(app, OperatingSystem.Windows);
        }
    }
}

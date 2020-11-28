namespace SpiderEye.Linux
{
    /// <summary>
    /// Provides Linux specific application methods.
    /// </summary>
    public static class LinuxApplication
    {
        private static GtkApplication? app;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Init()
        {
            app = new GtkApplication();
            Application.Register(app, OperatingSystem.Linux);
        }
    }
}

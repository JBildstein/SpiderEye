using SpiderEye.Linux.Interop;

namespace SpiderEye.Linux
{
    /// <summary>
    /// Provides Linux specific application methods.
    /// </summary>
    public static class LinuxApplication
    {
        /// <summary>
        /// Gets or sets the application ID for the status icon.
        /// </summary>
        public static string? ApplicationId { get; set; }

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

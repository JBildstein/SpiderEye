using System;
using System.Reflection;

namespace SpiderEye.Configuration
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the scripting interface between browser and window is enabled. Default is true.
        /// </summary>
        public bool EnableScriptInterface { get; set; }

        /// <summary>
        /// Gets or sets the start page url. Default is "index.html".
        /// </summary>
        public string StartPageUrl { get; set; }

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
        /// Gets or sets the window configuration.
        /// </summary>
        public WindowConfiguration Window
        {
            get { return window; }
            set { window = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        private WindowConfiguration window;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        public AppConfiguration()
        {
            Window = new WindowConfiguration();

            EnableScriptInterface = true;
            StartPageUrl = "index.html";
            ExternalHost = null;
            ContentFolder = "App";
            ContentAssembly = Assembly.GetEntryAssembly();
            ForceWindowsLegacyWebview = false;
        }
    }
}

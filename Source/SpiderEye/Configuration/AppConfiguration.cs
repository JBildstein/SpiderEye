using System;

namespace SpiderEye.Configuration
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Gets or sets the window configuration.
        /// </summary>
        public WindowConfiguration Window
        {
            get { return window; }
            set { window = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        /// <summary>
        /// Gets or sets the server configuration.
        /// </summary>
        public ServerConfiguration Server
        {
            get { return server; }
            set { server = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        private WindowConfiguration window;
        private ServerConfiguration server;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfiguration"/> class.
        /// </summary>
        public AppConfiguration()
        {
            Window = new WindowConfiguration();
            Server = new ServerConfiguration();
        }
    }
}

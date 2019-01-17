using System.Reflection;

namespace SpiderEye.Configuration
{
    /// <summary>
    /// Server configuration.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the internal server is started or not. Default is true.
        /// </summary>
        public bool UseInternalServer { get; set; }

        /// <summary>
        /// Gets or sets the start page url. Default is "index.html".
        /// </summary>
        public string StartPageUrl { get; set; }

        /// <summary>
        /// Gets or sets the host url. Default is null.
        /// If it's not set, it'll use the internal servers address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the internal servers localhost port. Default is null.
        /// If it's not set, it'll search for a free one.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the folder path where the embedded files are. Default is "App".
        /// </summary>
        public string ContentFolder { get; set; }

        /// <summary>
        /// Gets or sets the assembly where the content files are embedded. Default is the entry assembly.
        /// </summary>
        public Assembly ContentAssembly { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration"/> class.
        /// </summary>
        public ServerConfiguration()
        {
            UseInternalServer = true;
            StartPageUrl = "index.html";
            ContentFolder = "App";
            Host = null;
            ContentAssembly = Assembly.GetEntryAssembly();
        }
    }
}

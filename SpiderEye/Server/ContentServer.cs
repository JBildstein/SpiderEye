using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace SpiderEye.Server
{
    /// <summary>
    /// A simple HTTP server to provide the embedded content of a SpiderEye app.
    /// </summary>
    public class ContentServer : IDisposable
    {
        /// <summary>
        /// Gets the servers host address.
        /// If no explicit port number was provided in the constructor,
        /// this value is only valid after calling <see cref="Start"/>.
        /// </summary>
        public string HostAddress
        {
            get;
            private set;
        }

        private readonly Assembly contentAssembly;
        private readonly HttpListener listener;
        private readonly Dictionary<string, string> fileMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServer"/> class.
        /// </summary>
        /// <param name="contentAssembly">The assembly that contains the content.</param>
        /// <param name="contentFolder">The folder path of the content.</param>
        public ContentServer(Assembly contentAssembly, string contentFolder)
            : this(contentAssembly, contentFolder, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServer"/> class.
        /// </summary>
        /// <param name="contentAssembly">The assembly that contains the content.</param>
        /// <param name="contentFolder">The folder path of the content.</param>
        /// <param name="port">The port number the server should listen to. If set to zero, it will be auto-assigned.</param>
        public ContentServer(Assembly contentAssembly, string contentFolder, int port)
        {
            this.contentAssembly = contentAssembly ?? throw new ArgumentNullException(nameof(contentAssembly));
            listener = new HttpListener();
            fileMap = CreateFileMap(contentAssembly, contentFolder);

            if (port != 0) { HostAddress = $"http://localhost:{port}/"; }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            if (HostAddress == null)
            {
                // potential racing condition but unlikely to happen
                // other process would have to use the same port between GetFreeTcpPort and listener.Start
                HostAddress = $"http://localhost:{GetFreeTcpPort()}/";
            }

            listener.Prefixes.Add(HostAddress);
            listener.Start();
            listener.BeginGetContext(ListenerCallback, listener);
        }

        /// <summary>
        /// Closes the server.
        /// </summary>
        public void Dispose()
        {
            listener.Close();
        }

        private static int GetFreeTcpPort()
        {
            TcpListener tcp = null;
            try
            {
                tcp = new TcpListener(IPAddress.Loopback, 0);
                tcp.Start();

                return ((IPEndPoint)tcp.LocalEndpoint).Port;
            }
            finally { tcp?.Stop(); }
        }

        private Dictionary<string, string> CreateFileMap(Assembly contentAssembly, string contentFolder)
        {
            contentFolder = contentFolder.Replace('\\', '/');

            string[] files = contentAssembly.GetManifestResourceNames();
            var dict = new Dictionary<string, string>();
            foreach (string file in files)
            {
                if (file.StartsWith(contentFolder))
                {
                    string key = file.Substring(contentFolder.Length)
                        .Replace('\\', '/')
                        .TrimStart('/')
                        .ToLower();

                    dict.Add(key, file);
                }
            }

            return dict;
        }

        private void ListenerCallback(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            listener.BeginGetContext(ListenerCallback, listener);

            try
            {
                string file = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
                if (!fileMap.TryGetValue(file, out file))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                using (var content = contentAssembly.GetManifestResourceStream(file))
                {
                    if (content != null)
                    {
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = MimeTypes.FindForFile(file);

                        using (var output = context.Response.OutputStream)
                        {
                            content.CopyTo(output);
                        }
                    }
                    else { context.Response.StatusCode = 404; }
                }
            }
            catch (FileNotFoundException) { context.Response.StatusCode = 404; }
            catch { context.Response.StatusCode = 500; }
            finally { context.Response.Close(); }
        }
    }
}

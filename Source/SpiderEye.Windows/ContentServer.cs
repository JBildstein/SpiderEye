using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using SpiderEye.Tools;

namespace SpiderEye
{
    /// <summary>
    /// A simple HTTP server to run inside a SpiderEye app.
    /// </summary>
    internal class ContentServer : IDisposable
    {
        /// <summary>
        /// Gets the servers host address.
        /// If no explicit port number was provided in the constructor,
        /// this value is only valid after calling <see cref="Start"/>.
        /// </summary>
        public string? HostAddress
        {
            get;
            private set;
        }

        private readonly HttpListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServer"/> class.
        /// </summary>
        public ContentServer()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServer"/> class.
        /// </summary>
        /// <param name="port">The port number the server should listen to. If set to zero or null, it will be auto-assigned.</param>
        public ContentServer(int? port)
        {
            listener = new HttpListener();

            if (port != null && port != 0) { HostAddress = $"http://localhost:{port}/"; }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        [MemberNotNull(nameof(HostAddress))]
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
            TcpListener? tcp = null;
            try
            {
                tcp = new TcpListener(IPAddress.Loopback, 0);
                tcp.Start();

                return ((IPEndPoint)tcp.LocalEndpoint).Port;
            }
            finally { tcp?.Stop(); }
        }

        private async void ListenerCallback(IAsyncResult result)
        {
            var listener = result.AsyncState as HttpListener;
            HttpListenerContext? context = null;

            try
            {
                listener!.BeginGetContext(ListenerCallback, listener);
                context = listener.EndGetContext(result);
            }
            catch { return; }

            try
            {
                if (context.Request.HttpMethod.ToUpper() == "GET" && context.Request.Url != null)
                {
                    using var stream = await Application.ContentProvider.GetStreamAsync(context.Request.Url);
                    if (stream != null)
                    {
                        context.Response.ContentType = MimeTypes.FindForUri(context.Request.Url);
                        using var responseStream = context.Response.OutputStream;
                        await stream.CopyToAsync(responseStream);
                    }
                    else { context.Response.StatusCode = 404; }
                }
                else { context.Response.StatusCode = 400; }
            }
            catch { context.Response.StatusCode = 500; }
            finally { context.Response.Close(); }
        }
    }
}

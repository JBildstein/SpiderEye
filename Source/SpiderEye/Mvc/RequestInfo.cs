using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using SpiderEye.Server;

namespace SpiderEye.Mvc
{
    /// <summary>
    /// Contains information about an HTTP request.
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// Gets the request URL.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// Gets the length in bytes of the body.
        /// </summary>
        public long? ContentLength { get; }

        /// <summary>
        /// Gets the MIME type of the body data included in the request.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// Gets the <see cref="Encoding"/> for this request.
        /// </summary>
        public Encoding ContentEncoding { get; }

        /// <summary>
        /// Gets the requests HTTP method.
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Gets a collection of header name/value pairs sent in the request.
        /// </summary>
        public NameValueCollection Headers { get; }

        /// <summary>
        /// Gets a value indicating whether the request has a body or not.
        /// </summary>
        public bool HasBody { get; }

        /// <summary>
        /// Gets a <see cref="Stream"/> that contains the body data.
        /// Note: you should dispose the stream after using it.
        /// </summary>
        public Stream Body
        {
            get { return request.InputStream; }
        }

        private readonly HttpListenerRequest request;

        internal RequestInfo(HttpListenerRequest request)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));

            Url = request.Url;
            ContentLength = request.HasEntityBody ? request.ContentLength64 : (long?)null;
            ContentType = request.ContentType;
            ContentEncoding = request.ContentEncoding;
            Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), request.HttpMethod, true);
            HasBody = request.HasEntityBody;
            Headers = request.Headers;
        }
    }
}

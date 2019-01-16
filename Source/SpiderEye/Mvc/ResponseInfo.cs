using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace SpiderEye.Mvc
{
    /// <summary>
    /// Contains information about an HTTP response.
    /// </summary>
    public class ResponseInfo
    {
        /// <summary>
        /// Gets or sets the MIME type of the content returned.
        /// </summary>
        public string ContentType
        {
            get { return response.ContentType; }
            set { response.ContentType = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Encoding"/> for this response.
        /// </summary>
        public Encoding ContentEncoding
        {
            get { return response.ContentEncoding; }
            set { response.ContentEncoding = value; }
        }

        /// <summary>
        /// Gets or sets the HTTP status code to be returned to the client.
        /// </summary>
        public int StatusCode
        {
            get { return response.StatusCode; }
            set { response.StatusCode = value; }
        }

        /// <summary>
        /// Gets a collection of header name/value pairs returned by the server.
        /// </summary>
        public NameValueCollection Headers
        {
            get { return response.Headers; }
        }

        private readonly HttpListenerResponse response;

        internal ResponseInfo(HttpListenerResponse response)
        {
            this.response = response ?? throw new ArgumentNullException(nameof(response));
        }

        /// <summary>
        /// Configures the response to redirect the client to the specified URL.
        /// </summary>
        /// <param name="url">The URL that the client should use to locate the requested resource.</param>
        public void Redirect(string url)
        {
            response.Redirect(url);
        }

        /// <summary>
        /// Adds the specified header and value to the HTTP headers for this response.
        /// </summary>
        /// <param name="name">The name of the HTTP header to set.</param>
        /// <param name="value">The value of the header.</param>
        public void AddHeader(string name, string value)
        {
            response.AddHeader(name, value);
        }
    }
}

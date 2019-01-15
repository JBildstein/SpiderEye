using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP PUT request.
    /// </summary>
    public sealed class HttpPutAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPutAttribute"/> class.
        /// </summary>
        public HttpPutAttribute()
            : base(HttpMethod.Put)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPutAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpPutAttribute(string route)
            : base(HttpMethod.Put, route)
        {
        }
    }
}

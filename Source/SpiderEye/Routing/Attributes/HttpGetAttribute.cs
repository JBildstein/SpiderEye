using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP GET request.
    /// </summary>
    public sealed class HttpGetAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpGetAttribute"/> class.
        /// </summary>
        public HttpGetAttribute()
            : base(HttpMethod.Get)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpGetAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpGetAttribute(string route)
            : base(HttpMethod.Get, route)
        {
        }
    }
}

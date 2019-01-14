using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP TRACE request.
    /// </summary>
    public sealed class HttpTraceAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTraceAttribute"/> class.
        /// </summary>
        public HttpTraceAttribute()
            : base(HttpMethod.Trace)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTraceAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpTraceAttribute(string route)
            : base(HttpMethod.Put, route)
        {
        }
    }
}

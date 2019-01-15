using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP HEAD request.
    /// </summary>
    public sealed class HttpHeadAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeadAttribute"/> class.
        /// </summary>
        public HttpHeadAttribute()
            : base(HttpMethod.Head)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHeadAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpHeadAttribute(string route)
            : base(HttpMethod.Head, route)
        {
        }
    }
}

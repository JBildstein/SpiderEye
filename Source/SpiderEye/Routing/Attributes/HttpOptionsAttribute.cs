using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP OPTIONS request.
    /// </summary>
    public sealed class HttpOptionsAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOptionsAttribute"/> class.
        /// </summary>
        public HttpOptionsAttribute()
            : base(HttpMethod.Options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpOptionsAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpOptionsAttribute(string route)
            : base(HttpMethod.Options, route)
        {
        }
    }
}

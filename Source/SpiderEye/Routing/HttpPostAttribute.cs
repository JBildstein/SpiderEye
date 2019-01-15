using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP POST request.
    /// </summary>
    public sealed class HttpPostAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostAttribute"/> class.
        /// </summary>
        public HttpPostAttribute()
            : base(HttpMethod.Post)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPostAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpPostAttribute(string route)
            : base(HttpMethod.Post, route)
        {
        }
    }
}

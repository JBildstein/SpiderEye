using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller which gets activated on an HTTP DELETE request.
    /// </summary>
    public sealed class HttpDeleteAttribute : HttpRouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDeleteAttribute"/> class.
        /// </summary>
        public HttpDeleteAttribute()
            : base(HttpMethod.Delete)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDeleteAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public HttpDeleteAttribute(string route)
            : base(HttpMethod.Delete, route)
        {
        }
    }
}

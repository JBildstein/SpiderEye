using System;
using SpiderEye.Server;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Marks a method as a route of a controller for the given HTTP method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class HttpRouteAttribute : Attribute
    {
        /// <summary>
        /// Gets the HTTP method this route is intended for.
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Gets the route name.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteAttribute"/> class.
        /// </summary>
        /// <param name="method">The HTTP method this route is intended for.</param>
        internal protected HttpRouteAttribute(HttpMethod method)
            : this(method, "[action]")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteAttribute"/> class.
        /// </summary>
        /// <param name="method">The HTTP method this route is intended for.</param>
        /// <param name="route">The route name.</param>
        internal protected HttpRouteAttribute(HttpMethod method, string route)
        {
            Method = method;
            Route = route ?? throw new ArgumentNullException(nameof(route));
        }
    }
}

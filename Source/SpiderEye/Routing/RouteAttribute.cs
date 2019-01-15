using System;

namespace SpiderEye.Routing
{
    /// <summary>
    /// Provides a custom route for a controller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RouteAttribute : Attribute
    {
        /// <summary>
        /// Gets the route name.
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttribute"/> class.
        /// </summary>
        public RouteAttribute()
            : this("[controller]")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteAttribute"/> class.
        /// </summary>
        /// <param name="route">The route name.</param>
        public RouteAttribute(string route)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));
        }
    }
}

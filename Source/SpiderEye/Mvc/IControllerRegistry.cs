using System;
using SpiderEye.Server;

namespace SpiderEye.Mvc
{
    /// <summary>
    /// Provides methods to register controllers.
    /// </summary>
    internal interface IControllerRegistry
    {
        /// <summary>
        /// Registers a controller of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the controller to register.</typeparam>
        void Register<T>() where T : Controller, new();

        /// <summary>
        /// Registers a controller with a factory method.
        /// </summary>
        /// <typeparam name="T">The type of the controller to register.</typeparam>
        /// <param name="controllerFactory">The factory to create a controller instance.</param>
        void Register<T>(Func<T> controllerFactory) where T : Controller;

        /// <summary>
        /// Tries to get the controller info for a given path.
        /// </summary>
        /// <param name="httpMethod">The HTTP method of the request.</param>
        /// <param name="path">The request path.</param>
        /// <param name="info">The controller info if found; null otherwise.</param>
        /// <returns>True if a controller was found for the given path; False otherwise.</returns>
        bool TryGetInfo(HttpMethod httpMethod, string path, out ControllerMethodInfo info);
    }
}

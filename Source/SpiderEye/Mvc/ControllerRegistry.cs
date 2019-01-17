using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SpiderEye.Routing;
using SpiderEye.Server;

namespace SpiderEye.Mvc
{
    /// <summary>
    /// Provides methods to register controllers.
    /// </summary>
    internal class ControllerRegistry : IControllerRegistry
    {
        private readonly Dictionary<string, ControllerMethodInfo> paths = new Dictionary<string, ControllerMethodInfo>();

        /// <summary>
        /// Registers a controller of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the controller to register.</typeparam>
        public void Register<T>()
            where T : Controller, new()
        {
            Register(() => Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Registers a controller with a factory method.
        /// </summary>
        /// <typeparam name="T">The type of the controller to register.</typeparam>
        /// <param name="controllerFactory">The factory to create a controller instance.</param>
        public void Register<T>(Func<T> controllerFactory)
            where T : Controller
        {
            if (controllerFactory == null) { throw new ArgumentNullException(nameof(controllerFactory)); }

            var type = typeof(T);
            string url = type.Name.ToLower();
            if (url.EndsWith("controller")) { url = url.Substring(0, url.Length - 10); }
            if (!url.EndsWith("/")) { url += "/"; }

            var attribute = type.GetCustomAttribute<RouteAttribute>(true);
            if (attribute != null)
            {
                url = attribute.Route.ToLower().Replace("[controller]", url);
            }

            var uri = new UriBuilder("http", "localhost", 0, url).Uri;
            AddMethods(type, uri, controllerFactory);
        }

        /// <summary>
        /// Tries to get the controller info for a given path.
        /// </summary>
        /// <param name="httpMethod">The HTTP method of the request.</param>
        /// <param name="path">The request path.</param>
        /// <param name="info">The controller info if found; null otherwise.</param>
        /// <returns>True if a controller was found for the given path; False otherwise.</returns>
        public bool TryGetInfo(HttpMethod httpMethod, string path, out ControllerMethodInfo info)
        {
            string key = $"{httpMethod.ToString().ToUpper()} {path.ToLower()}";
            return paths.TryGetValue(key, out info);
        }

        private void AddMethods(Type type, Uri controllerUri, Func<Controller> factory)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                HttpMethod httpMethod = HttpMethod.Get;
                string url = method.Name.ToLower();

                var attributes = method.GetCustomAttributes<HttpRouteAttribute>(true);
                if (attributes.Any())
                {
                    if (attributes.Skip(1).Any())
                    {
                        string message = $"Multiple HttpRoute attributes applied to method \"{type.Name}.{method.Name}\"";
                        throw new InvalidOperationException(message);
                    }

                    var attribute = attributes.First();
                    httpMethod = attribute.Method;
                    url = attribute.Route.ToLower().Replace("[action]", url);
                }

                string fullUrl = new Uri(controllerUri, url).GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
                string key = $"{httpMethod.ToString().ToUpper()} {fullUrl}";
                if (paths.ContainsKey(key))
                {
                    string message = $"Path {key} was already added. Source: \"{type.Name}.{method.Name}\"";
                    throw new InvalidOperationException(message);
                }

                var parameters = GetParameterInfos(method);
                var info = new ControllerMethodInfo(httpMethod, fullUrl, method, parameters, factory);
                paths.Add(key, info);
            }
        }

        private MethodParameterInfo[] GetParameterInfos(MethodInfo method)
        {
            bool hasBody = false;
            var parameters = method.GetParameters();
            var result = new MethodParameterInfo[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var type = parameters[i].ParameterType;
                var source = MethodParameterSource.Body;
                if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime))
                {
                    source = MethodParameterSource.Query;
                }
                else if (hasBody)
                {
                    string message = $"Method \"{method.DeclaringType.Name}.{method.Name}\" has multiple body parameters: \"{parameters[i].Name}\"";
                    throw new InvalidOperationException(message);
                }
                else { hasBody = true; }

                // TODO: add parameters with a source of Path
                result[i] = new MethodParameterInfo(source, parameters[i].Name, type);
            }

            return result;
        }
    }
}

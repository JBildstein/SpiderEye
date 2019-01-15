using System;
using System.Reflection;
using SpiderEye.Server;

namespace SpiderEye.Mvc
{
    internal class ControllerMethodInfo
    {
        public HttpMethod HttpMethod { get; }
        public string Url { get; }
        public MethodInfo Method { get; }
        public MethodParameterInfo[] Parameters { get; }
        public Func<Controller> Factory { get; }

        public ControllerMethodInfo(
            HttpMethod httpMethod,
            string url,
            MethodInfo method,
            MethodParameterInfo[] parameters,
            Func<Controller> factory)
        {
            HttpMethod = httpMethod;
            Url = url;
            Method = method;
            Parameters = parameters;
            Factory = factory;
        }
    }
}

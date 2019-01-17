using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SpiderEye.Json;
using SpiderEye.Mvc;

namespace SpiderEye.Server.Middleware
{
    internal class ControllerMiddleware : IMiddleware
    {
        private readonly IControllerRegistry controllerRegistry;

        public ControllerMiddleware(IControllerRegistry controllerRegistry)
        {
            this.controllerRegistry = controllerRegistry ?? throw new ArgumentNullException(nameof(controllerRegistry));
        }

        public async Task InvokeAsync(HttpListenerContext context, Func<Task> next)
        {
            var httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), context.Request.HttpMethod, true);
            string path = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
            if (controllerRegistry.TryGetInfo(httpMethod, path, out ControllerMethodInfo controllerInfo))
            {
                context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                context.Response.ContentType = MimeTypes.Json;
                context.Response.StatusCode = 200;

                using (var controller = controllerInfo.Factory())
                {
                    controller.Request = new RequestInfo(context.Request);
                    controller.Response = new ResponseInfo(context.Response);

                    object[] parameters = await ResolveParameters(controllerInfo, context.Request);
                    object controllerResult = controllerInfo.Method.Invoke(controller, parameters);
                    await WriteControllerResult(controllerResult, controllerInfo.Method.ReturnType, context.Response);
                }
            }
            else { await next(); }
        }

        private async Task<object[]> ResolveParameters(ControllerMethodInfo info, HttpListenerRequest request)
        {
            var bodyParameter = info.Parameters.FirstOrDefault(t => t.Source == MethodParameterSource.Body);
            object body = null;
            if (bodyParameter != null && request.HasEntityBody)
            {
                if (request.ContentType != MimeTypes.Json)
                {
                    throw new NotSupportedException("Only JSON body is supported");
                }

                using (var bodyStream = request.InputStream)
                using (var reader = new StreamReader(bodyStream, request.ContentEncoding))
                {
                    string json = await reader.ReadToEndAsync();
                    body = JsonConvert.Deserialize(json, bodyParameter.Type);
                }
            }

            object[] result = new object[info.Parameters.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var parameter = info.Parameters[i];
                var source = parameter.Source;
                switch (source)
                {
                    case MethodParameterSource.Body:
                        result[i] = body;
                        break;

                    case MethodParameterSource.Query:
                        string queryValue = request.QueryString.Get(parameter.Name);
                        result[i] = CastParameter(queryValue, parameter.Type, parameter.Name);
                        break;

                    case MethodParameterSource.Path:
                        throw new NotImplementedException(); // TODO: support path parameters

                    default:
                        throw new InvalidOperationException($"Unknown method parameter source \"{source}\"");
                }
            }

            return result;
        }

        private object CastParameter(string value, Type type, string name)
        {
            if (ParameterCastMap.TryGetValue(type, out Func<string, object> map))
            {
                return map(value);
            }
            else
            {
                Type valueType = null;
                if (type.IsArray) { valueType = type.GetElementType(); }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    valueType = type.GetGenericArguments()[0];
                }

                if (valueType != null && ParameterCastMap.TryGetValue(valueType, out map))
                {
                    string[] values = value.Split(',');
                    object[] result = new object[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        result[i] = map(values[i]);
                    }

                    return result;
                }
                else { throw new InvalidOperationException($"Invalid type {type.Name} for parameter \"{name}\""); }
            }
        }

        private async Task WriteControllerResult(object result, Type returnType, HttpListenerResponse response)
        {
            if (returnType == typeof(void)) { return; }

            if (result != null)
            {
                if (returnType == typeof(Task))
                {
                    await (Task)result;
                    return;
                }
                else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    await (Task)result;
                    result = returnType.GetProperty("Result").GetValue(result);
                }

                string json = JsonConvert.Serialize(result);
                response.ContentEncoding = response.ContentEncoding ?? Encoding.UTF8;
                using (var stream = response.OutputStream)
                using (var writer = new StreamWriter(stream, response.ContentEncoding))
                {
                    await writer.WriteAsync(json);
                }
            }
        }

        private static readonly Dictionary<Type, Func<string, object>> ParameterCastMap = new Dictionary<Type, Func<string, object>>
        {
            { typeof(byte), s => byte.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(sbyte), s => sbyte.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(short), s => short.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(ushort), s => ushort.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(int), s => int.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(uint), s => uint.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(long), s => long.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(ulong), s => ulong.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(float), s => float.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(double), s => double.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(decimal), s => decimal.Parse(s, CultureInfo.InvariantCulture) },
            { typeof(bool), s => s?.ToLower() == "true" ? true : false },
            { typeof(string), s => s },
            { typeof(DateTime), s => DateTime.ParseExact(s, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime() },
        };
    }
}

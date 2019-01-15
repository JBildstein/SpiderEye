using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpiderEye.Tools.Json;

namespace SpiderEye.Server
{
    /// <summary>
    /// A simple HTTP server to provide the embedded content of a SpiderEye app and serve controllers.
    /// </summary>
    public class ContentServer : IDisposable
    {
        /// <summary>
        /// Gets the servers host address.
        /// If no explicit port number was provided in the constructor,
        /// this value is only valid after calling <see cref="Start"/>.
        /// </summary>
        public string HostAddress
        {
            get;
            private set;
        }

        private readonly Assembly contentAssembly;
        private readonly HttpListener listener;
        private readonly Dictionary<string, string> fileMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServer"/> class.
        /// </summary>
        /// <param name="contentAssembly">The assembly that contains the content.</param>
        /// <param name="contentFolder">The folder path of the content.</param>
        public ContentServer(Assembly contentAssembly, string contentFolder)
            : this(contentAssembly, contentFolder, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentServer"/> class.
        /// </summary>
        /// <param name="contentAssembly">The assembly that contains the content.</param>
        /// <param name="contentFolder">The folder path of the content.</param>
        /// <param name="port">The port number the server should listen to. If set to zero or null, it will be auto-assigned.</param>
        public ContentServer(Assembly contentAssembly, string contentFolder, int? port)
        {
            this.contentAssembly = contentAssembly ?? throw new ArgumentNullException(nameof(contentAssembly));
            listener = new HttpListener();
            fileMap = CreateFileMap(contentAssembly, contentFolder);

            if (port != null && port != 0) { HostAddress = $"http://localhost:{port}/"; }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            if (HostAddress == null)
            {
                // potential racing condition but unlikely to happen
                // other process would have to use the same port between GetFreeTcpPort and listener.Start
                HostAddress = $"http://localhost:{GetFreeTcpPort()}/";
            }

            listener.Prefixes.Add(HostAddress);
            listener.Start();
            listener.BeginGetContext(ListenerCallback, listener);
        }

        /// <summary>
        /// Closes the server.
        /// </summary>
        public void Dispose()
        {
            listener.Close();
        }

        private static int GetFreeTcpPort()
        {
            TcpListener tcp = null;
            try
            {
                tcp = new TcpListener(IPAddress.Loopback, 0);
                tcp.Start();

                return ((IPEndPoint)tcp.LocalEndpoint).Port;
            }
            finally { tcp?.Stop(); }
        }

        private Dictionary<string, string> CreateFileMap(Assembly contentAssembly, string contentFolder)
        {
            contentFolder = contentFolder.Replace('\\', '/');

            string[] files = contentAssembly.GetManifestResourceNames();
            var dict = new Dictionary<string, string>();
            foreach (string file in files)
            {
                if (file.StartsWith(contentFolder))
                {
                    string key = file.Substring(contentFolder.Length)
                        .Replace('\\', '/')
                        .TrimStart('/')
                        .ToLower();

                    dict.Add(key, file);
                }
            }

            return dict;
        }

        private async void ListenerCallback(IAsyncResult result)
        {
            var listener = result.AsyncState as HttpListener;
            HttpListenerContext context = null;
            try
            {
                listener.BeginGetContext(ListenerCallback, listener);
                context = listener.EndGetContext(result);
            }
            catch { return; }

            try
            {
                var httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), context.Request.HttpMethod, true);
                string path = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
                if (httpMethod == HttpMethod.Get && fileMap.TryGetValue(path, out string file))
                {
                    using (var content = contentAssembly.GetManifestResourceStream(file))
                    {
                        if (content != null)
                        {
                            context.Response.StatusCode = 200;
                            context.Response.ContentType = MimeTypes.FindForFile(file);

                            using (var output = context.Response.OutputStream)
                            {
                                await content.CopyToAsync(output);
                            }
                        }
                        else { context.Response.StatusCode = 404; }
                    }
                }
                else
                {
                    if (ControllerRegistry.TryGetInfo(httpMethod, path, out ControllerMethodInfo controllerInfo))
                    {
                        context.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                        context.Response.ContentType = MimeTypes.Json;
                        context.Response.StatusCode = 200;

                        var controller = controllerInfo.Factory();
                        object[] parameters = await ResolveParameters(controllerInfo, context.Request);
                        object controllerResult = controllerInfo.Method.Invoke(controller, parameters);
                        await WriteControllerResult(controllerResult, controllerInfo.Method.ReturnType, context.Response);
                    }
                    else { context.Response.StatusCode = 404; }
                }
            }
            catch (FileNotFoundException) { context.Response.StatusCode = 404; }
            catch { context.Response.StatusCode = 500; }
            finally { context.Response.Close(); }
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
                using (var stream = response.OutputStream)
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
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
            { typeof(DateTime), s => DateTime.ParseExact(s, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) },
        };
    }
}

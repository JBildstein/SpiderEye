using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace SpiderEye.Server.Middleware
{
    internal class EmbeddedFileMiddleware : IMiddleware
    {
        private readonly Assembly contentAssembly;
        private readonly Dictionary<string, string> fileMap;

        public EmbeddedFileMiddleware(Assembly contentAssembly, string contentFolder)
        {
            this.contentAssembly = contentAssembly ?? throw new ArgumentNullException(nameof(contentAssembly));
            fileMap = CreateFileMap(contentAssembly, contentFolder);
        }

        public async Task InvokeAsync(HttpListenerContext context, Func<Task> next)
        {
            var httpMethod = (HttpMethod)Enum.Parse(typeof(HttpMethod), context.Request.HttpMethod, true);
            string path = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
            if (httpMethod == HttpMethod.Get && fileMap.TryGetValue(path, out string file))
            {
                try
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
                catch (FileNotFoundException) { context.Response.StatusCode = 404; }
            }
            else { await next(); }
        }

        private Dictionary<string, string> CreateFileMap(Assembly contentAssembly, string contentFolder)
        {
            contentFolder = NormalizePath(contentFolder);

            string[] files = contentAssembly.GetManifestResourceNames();
            var dict = new Dictionary<string, string>();
            foreach (string file in files)
            {
                string key = NormalizePath(file);
                if (key.StartsWith(contentFolder))
                {
                    dict.Add(key.Substring(contentFolder.Length).TrimStart('/'), file);
                }
            }

            return dict;
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/')
                    .TrimStart('/')
                    .ToLower();
        }
    }
}

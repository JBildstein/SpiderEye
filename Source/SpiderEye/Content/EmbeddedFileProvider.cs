using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SpiderEye.Content
{
    internal class EmbeddedFileProvider : IContentProvider
    {
        private readonly Assembly contentAssembly;
        private readonly Dictionary<string, string> fileMap;

        public EmbeddedFileProvider(Assembly contentAssembly, string contentFolder)
        {
            this.contentAssembly = contentAssembly ?? throw new ArgumentNullException(nameof(contentAssembly));
            fileMap = CreateFileMap(contentAssembly, contentFolder);
        }

        public Task<Stream> GetStreamAsync(Uri uri)
        {
            Stream result = null;
            string path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
            if (fileMap.TryGetValue(path, out string file))
            {
                try { result = contentAssembly.GetManifestResourceStream(file); }
                catch (FileNotFoundException) { result = null; }
            }

            return Task.FromResult(result);
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SpiderEye.Tools;

namespace SpiderEye
{
    /// <summary>
    /// Content provider for files that are embedded in an assembly.
    /// </summary>
    public class EmbeddedContentProvider : IContentProvider
    {
        private readonly Assembly contentAssembly;
        private readonly string notFoundFile;
        private readonly Dictionary<string, string> fileMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedContentProvider"/> class.
        /// </summary>
        /// <param name="contentFolder">Gets or sets the folder path where the embedded files are.</param>
        /// <param name="notFoundPath">The path of the file which should be returned if the requested file is not found.</param>
        public EmbeddedContentProvider(string contentFolder, string notFoundPath = null)
            : this(contentFolder, Assembly.GetCallingAssembly(), notFoundPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedContentProvider"/> class.
        /// </summary>
        /// <param name="contentFolder">Gets or sets the folder path where the embedded files are.</param>
        /// <param name="contentAssembly">Gets or sets the assembly where the content files are embedded.</param>
        /// <param name="notFoundPath">The path of the file which should be returned if the requested file is not found.</param>
        public EmbeddedContentProvider(string contentFolder, Assembly contentAssembly, string notFoundPath = null)
        {
            if (contentFolder == null) { throw new ArgumentNullException(nameof(contentFolder)); }

            this.contentAssembly = contentAssembly ?? throw new ArgumentNullException(nameof(contentAssembly));
            fileMap = CreateFileMap(contentAssembly, contentFolder);

            if (notFoundPath != null)
            {
                fileMap.TryGetValue(NormalizePath(notFoundPath), out notFoundFile);
            }
        }

        /// <inheritdoc/>
        public Task<Stream> GetStreamAsync(Uri uri)
        {
            Stream result = null;
            var file = ResolveFile(uri);
            if (file != null)
            {
                try { result = contentAssembly.GetManifestResourceStream(file); }
                catch (FileNotFoundException) { result = null; }
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public string GetMimeType(Uri uri)
        {
            return MimeTypes.FindForFile(ResolveFile(uri));
        }

        private string ResolveFile(Uri uri)
        {
            var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).ToLower();
            return fileMap.TryGetValue(path, out var file)
                ? file
                : notFoundFile;
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

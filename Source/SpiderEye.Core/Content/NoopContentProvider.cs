using System;
using System.IO;
using System.Threading.Tasks;
using SpiderEye.Tools;

namespace SpiderEye
{
    /// <summary>
    /// Content provider that does nothing.
    /// </summary>
    internal sealed class NoopContentProvider : IContentProvider
    {
        /// <summary>
        /// An instance of a <see cref="NoopContentProvider"/>.
        /// </summary>
        public static readonly NoopContentProvider Instance = new NoopContentProvider();

        /// <inheritdoc/>
        public Task<Stream> GetStreamAsync(Uri uri)
        {
            return Task.FromResult<Stream>(null);
        }

        /// <inheritdoc/>
        public string GetMimeType(Uri uri) => MimeTypes.FindForUri(uri);
    }
}

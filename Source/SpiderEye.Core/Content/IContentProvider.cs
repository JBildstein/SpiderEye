using System;
using System.IO;
using System.Threading.Tasks;

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to get content from an URI.
    /// </summary>
    public interface IContentProvider
    {
        /// <summary>
        /// Gets the <see cref="Stream"/> to a resource given an URI.
        /// </summary>
        /// <param name="uri">The URI of the resource.</param>
        /// <returns>A Task with the <see cref="Stream"/> to the resource or null if not found.</returns>
        Task<Stream?> GetStreamAsync(Uri uri);
    }
}

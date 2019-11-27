using System;

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to check and manipulate URIs before they are loaded.
    /// </summary>
    public interface IUriWatcher
    {
        /// <summary>
        /// Checks the given URI before it's loaded.
        /// </summary>
        /// <param name="uri">The URI to check.</param>
        /// <returns>The final URI to load.</returns>
        Uri CheckUri(Uri uri);
    }
}

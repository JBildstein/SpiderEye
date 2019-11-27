using System;

namespace SpiderEye
{
    /// <summary>
    /// An URI watcher that does nothing.
    /// </summary>
    internal sealed class NoopUriWatcher : IUriWatcher
    {
        /// <summary>
        /// An instance of a <see cref="NoopUriWatcher"/>.
        /// </summary>
        public static readonly NoopUriWatcher Instance = new NoopUriWatcher();

        /// <inheritdoc/>
        public Uri CheckUri(Uri uri)
        {
            return uri;
        }
    }
}

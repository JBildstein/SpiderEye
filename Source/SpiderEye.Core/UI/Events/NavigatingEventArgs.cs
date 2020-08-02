using System;

namespace SpiderEye
{
    /// <summary>
    /// A delegate for a navigating event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void NavigatingEventHandler(object sender, NavigatingEventArgs e);

    /// <summary>
    /// Represents the event arguments for a navigating event.
    /// </summary>
    public sealed class NavigatingEventArgs : CancelableEventArgs
    {
        /// <summary>
        /// Gets the URL to which the webview is navigating.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatingEventArgs"/> class.
        /// </summary>
        /// <param name="url">The loaded URL.</param>
        internal NavigatingEventArgs(Uri url)
        {
            Url = url;
        }
    }
}

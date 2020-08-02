using System;

namespace SpiderEye
{
    /// <summary>
    /// A delegate for a page load event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void PageLoadEventHandler(object sender, PageLoadEventArgs e);

    /// <summary>
    /// Represents the event arguments for a page load event.
    /// </summary>
    public sealed class PageLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the loaded URL.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// Gets a value indicating whether the page loaded or not.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageLoadEventArgs"/> class.
        /// </summary>
        /// <param name="url">The loaded URL.</param>
        /// <param name="success">A value indicating whether the page loaded or not.</param>
        internal PageLoadEventArgs(Uri url, bool success)
        {
            Url = url;
            Success = success;
        }
    }
}

using System;

namespace SpiderEye.UI
{
    /// <summary>
    /// A delegate for a page load event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void PageLoadEventHandler(object sender, PageLoadEventArgs e);

    /// <summary>
    /// Represents the event arguments for page load event.
    /// </summary>
    public sealed class PageLoadEventArgs : EventArgs
    {
        /// <summary>
        /// An instance of the <see cref="PageLoadEventArgs"/> that states success.
        /// </summary>
        internal static readonly PageLoadEventArgs Successful = new PageLoadEventArgs(true);

        /// <summary>
        /// An instance of the <see cref="PageLoadEventArgs"/> that states failure.
        /// </summary>
        internal static readonly PageLoadEventArgs Failed = new PageLoadEventArgs(false);

        /// <summary>
        /// Gets a value indicating whether the page loaded or not.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageLoadEventArgs"/> class.
        /// </summary>
        /// <param name="success">A value indicating whether the page loaded or not.</param>
        internal PageLoadEventArgs(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Gets the <see cref="PageLoadEventArgs"/> for the given state.
        /// </summary>
        /// <param name="success">A value indicating whether the page loaded or not.</param>
        /// <returns>The appropriate <see cref="PageLoadEventArgs"/>.</returns>
        internal static PageLoadEventArgs GetFor(bool success)
        {
            return success ? Successful : Failed;
        }
    }
}

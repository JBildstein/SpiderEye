using System;

namespace SpiderEye
{
    /// <summary>
    /// A delegate for an event of a cancelable operation.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void CancelableEventHandler(object sender, CancelableEventArgs e);

    /// <summary>
    /// Represents the event arguments for an event of a cancelable operation.
    /// </summary>
    public sealed class CancelableEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation should be canceled or not.
        /// </summary>
        public bool Cancel { get; set; }
    }
}

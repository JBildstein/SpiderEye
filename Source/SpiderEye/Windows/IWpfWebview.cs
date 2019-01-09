using System;

namespace SpiderEye.Windows
{
    /// <summary>
    /// Represents a WPF webview.
    /// </summary>
    internal interface IWpfWebview : IWebview
    {
        /// <summary>
        /// Fires when the title within the webview was changed.
        /// </summary>
        event EventHandler<string> TitleChanged;

        /// <summary>
        /// Gets the webview control.
        /// </summary>
        IDisposable Control { get; }
    }
}

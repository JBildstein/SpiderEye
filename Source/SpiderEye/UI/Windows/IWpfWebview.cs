using System;

namespace SpiderEye.UI.Windows
{
    /// <summary>
    /// Represents a WPF webview.
    /// </summary>
    internal interface IWpfWebview : IWebview
    {
        /// <summary>
        /// Gets the webview control.
        /// </summary>
        IDisposable Control { get; }
    }
}

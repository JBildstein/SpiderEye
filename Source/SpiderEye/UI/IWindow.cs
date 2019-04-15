using System;
using SpiderEye.Bridge;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a window.
    /// </summary>
    public interface IWindow : IDisposable
    {
        /// <summary>
        /// Fires once the content in the webview has loaded.
        /// </summary>
        event PageLoadEventHandler PageLoaded;

        /// <summary>
        /// Fires before the window gets closed.
        /// </summary>
        event CancelableEventHandler Closing;

        /// <summary>
        /// Fires after the window has closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets a bridge to the webview.
        /// </summary>
        IWebviewBridge Bridge { get; }

        /// <summary>
        /// Shows this window.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes this window.
        /// </summary>
        void Close();

        /// <summary>
        /// Sets the window state.
        /// </summary>
        /// <param name="state">The state to set.</param>
        void SetWindowState(WindowState state);

        /// <summary>
        /// Sets the icon for this window.
        /// </summary>
        /// <param name="icon">The icon for this window.</param>
        void SetIcon(Icon icon);

        /// <summary>
        /// Loads the given URL.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        void LoadUrl(string url);
    }
}

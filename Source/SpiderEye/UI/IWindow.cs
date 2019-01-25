using System;

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
        event EventHandler PageLoaded;

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        string Title { get; set; }

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
        /// Loads the given URL.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        void LoadUrl(string url);
    }
}

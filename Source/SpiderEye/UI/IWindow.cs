using System;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a window.
    /// </summary>
    public interface IWindow : IDisposable
    {
        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets the window width.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the window height.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the background color of the window.
        /// </summary>
        string BackgroundColor { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the window can be resized or not.
        /// </summary>
        bool CanResize { get; set; }

        /// <summary>
        /// Shows this window.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes this window.
        /// </summary>
        void Close();

        /// <summary>
        /// Resizes the window.
        /// </summary>
        /// <param name="width">The wanted width.</param>
        /// <param name="height">The wanted height.</param>
        void Resize(int width, int height);

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

using System;
using SpiderEye.Configuration;

namespace SpiderEye.UI
{
    /// <summary>
    /// Provides methods to create windows and dialogs.
    /// </summary>
    public interface IWindowFactory
    {
        /// <summary>
        /// Creates a new window.
        /// </summary>
        /// <param name="config">The window configuration.</param>
        /// <returns>The created window.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is null.</exception>
        IWindow CreateWindow(WindowConfiguration config);

        /// <summary>
        /// Creates a new message box.
        /// </summary>
        /// <returns>The created message box.</returns>
        IMessageBox CreateMessageBox();

        /// <summary>
        /// Creates a new save file dialog.
        /// </summary>
        /// <returns>The created save file dialog.</returns>
        ISaveFileDialog CreateSaveFileDialog();

        /// <summary>
        /// Creates a new open file dialog.
        /// </summary>
        /// <returns>The created open file dialog.</returns>
        IOpenFileDialog CreateOpenFileDialog();
    }
}

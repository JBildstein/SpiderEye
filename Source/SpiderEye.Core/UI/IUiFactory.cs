using SpiderEye.Bridge;

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to various UI elements like windows and dialogs.
    /// </summary>
    internal interface IUiFactory
    {
        /// <summary>
        /// Creates a new window.
        /// </summary>
        /// <param name="config">The window configuration.</param>
        /// <param name="bridge">The bridge to the webview.</param>
        /// <returns>The created window.</returns>
        IWindow CreateWindow(WindowConfiguration config, WebviewBridge bridge);

        /// <summary>
        /// Creates a new status icon.
        /// </summary>
        /// <param name="title">The status icon title.</param>
        /// <returns>The created status icon.</returns>
        IStatusIcon CreateStatusIcon(string title);

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

        /// <summary>
        /// Creates a new folder select dialog.
        /// </summary>
        /// <returns>The created folder select dialog.</returns>
        IFolderSelectDialog CreateFolderSelectDialog();

        /// <summary>
        /// Creates a new menu.
        /// </summary>
        /// <returns>The created menu.</returns>
        IMenu CreateMenu();

        /// <summary>
        /// Creates a new label menu item.
        /// </summary>
        /// <param name="label">The label of the menu item.</param>
        /// <returns>The created label menu item.</returns>
        ILabelMenuItem CreateLabelMenu(string label);

        /// <summary>
        /// Creates a new menu separator item.
        /// </summary>
        /// <returns>The created menu separator item.</returns>
        IMenuItem CreateMenuSeparator();
    }
}

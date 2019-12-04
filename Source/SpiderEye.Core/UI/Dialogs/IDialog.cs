namespace SpiderEye
{
    /// <summary>
    /// Represents a dialog.
    /// </summary>
    internal interface IDialog
    {
        /// <summary>
        /// Gets or sets the title of the dialog.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>The user selection.</returns>
        DialogResult Show();

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <returns>The user selection.</returns>
        DialogResult Show(IWindow parent);
    }
}

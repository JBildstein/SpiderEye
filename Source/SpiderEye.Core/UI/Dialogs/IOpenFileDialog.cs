namespace SpiderEye
{
    /// <summary>
    /// Represents an open file dialog.
    /// </summary>
    internal interface IOpenFileDialog : IFileDialog
    {
        /// <summary>
        /// Gets or sets a value indicating whether to allow multiple selections.
        /// </summary>
        bool Multiselect { get; set; }

        /// <summary>
        /// Gets the files selected by the user.
        /// </summary>
        string[] SelectedFiles { get; }
    }
}

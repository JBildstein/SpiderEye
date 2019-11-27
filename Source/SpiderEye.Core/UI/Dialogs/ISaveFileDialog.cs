namespace SpiderEye
{
    /// <summary>
    /// Represents a save file dialog.
    /// </summary>
    internal interface ISaveFileDialog : IFileDialog
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show a prompt if the file already exists.
        /// </summary>
        bool OverwritePrompt { get; set; }
    }
}

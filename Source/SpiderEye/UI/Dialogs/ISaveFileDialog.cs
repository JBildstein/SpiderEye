namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a save file dialog.
    /// </summary>
    public interface ISaveFileDialog : IFileDialog
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show a prompt if the file already exists.
        /// </summary>
        bool OverwritePrompt { get; set; }
    }
}

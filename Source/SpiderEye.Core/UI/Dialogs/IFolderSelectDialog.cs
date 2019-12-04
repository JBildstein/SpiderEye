namespace SpiderEye
{
    /// <summary>
    /// Represents a dialog to select a folder.
    /// </summary>
    internal interface IFolderSelectDialog : IDialog
    {
        /// <summary>
        /// Gets or sets the selected path.
        /// </summary>
        string SelectedPath { get; set; }
    }
}

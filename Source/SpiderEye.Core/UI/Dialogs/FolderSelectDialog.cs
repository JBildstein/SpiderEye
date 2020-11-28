namespace SpiderEye
{
    /// <summary>
    /// Represents a folder select dialog.
    /// </summary>
    public sealed class FolderSelectDialog : Dialog
    {
        /// <summary>
        /// Gets or sets the selected path.
        /// </summary>
        public string? SelectedPath
        {
            get { return NativeFolderSelectDialog.SelectedPath; }
            set { NativeFolderSelectDialog.SelectedPath = value; }
        }

        /// <summary>
        /// Gets the native folder select dialog.
        /// </summary>
        internal IFolderSelectDialog NativeFolderSelectDialog { get; }

        /// <inheritdoc/>
        internal override IDialog NativeDialog
        {
            get { return NativeFolderSelectDialog; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderSelectDialog"/> class.
        /// </summary>
        public FolderSelectDialog()
        {
            NativeFolderSelectDialog = Application.Factory.CreateFolderSelectDialog();
        }
    }
}

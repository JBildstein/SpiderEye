namespace SpiderEye
{
    /// <summary>
    /// Represents a save file dialog.
    /// </summary>
    public sealed class SaveFileDialog : FileDialog
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show a prompt if the file already exists.
        /// </summary>
        public bool OverwritePrompt
        {
            get { return NativeSaveFileDialog.OverwritePrompt; }
            set { NativeSaveFileDialog.OverwritePrompt = value; }
        }

        /// <summary>
        /// Gets the native save file dialog.
        /// </summary>
        internal ISaveFileDialog NativeSaveFileDialog { get; }

        /// <inheritdoc/>
        internal override IFileDialog NativeFileDialog
        {
            get { return NativeSaveFileDialog; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileDialog"/> class.
        /// </summary>
        public SaveFileDialog()
        {
            NativeSaveFileDialog = Application.Factory.CreateSaveFileDialog();
        }
    }
}

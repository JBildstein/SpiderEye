namespace SpiderEye
{
    /// <summary>
    /// Represents an open file dialog.
    /// </summary>
    public sealed class OpenFileDialog : FileDialog
    {
        /// <summary>
        /// Gets or sets a value indicating whether to allow multiple selections.
        /// </summary>
        public bool Multiselect
        {
            get { return NativeOpenFileDialog.Multiselect; }
            set { NativeOpenFileDialog.Multiselect = value; }
        }

        /// <summary>
        /// Gets the files selected by the user.
        /// </summary>
        public string[]? SelectedFiles
        {
            get { return NativeOpenFileDialog.SelectedFiles; }
        }

        /// <summary>
        /// Gets the native open file dialog.
        /// </summary>
        internal IOpenFileDialog NativeOpenFileDialog { get; }

        /// <inheritdoc/>
        internal override IFileDialog NativeFileDialog
        {
            get { return NativeOpenFileDialog; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileDialog"/> class.
        /// </summary>
        public OpenFileDialog()
        {
            NativeOpenFileDialog = Application.Factory.CreateOpenFileDialog();
        }
    }
}

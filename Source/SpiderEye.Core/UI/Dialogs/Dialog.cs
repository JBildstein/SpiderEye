namespace SpiderEye
{
    /// <summary>
    /// Represents a dialog.
    /// </summary>
    public abstract class Dialog
    {
        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        public string? Title
        {
            get { return NativeDialog.Title; }
            set { NativeDialog.Title = value; }
        }

        /// <summary>
        /// Gets the native dialog.
        /// </summary>
        internal abstract IDialog NativeDialog { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        private protected Dialog()
        {
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>The user selection.</returns>
        public DialogResult Show()
        {
            return NativeDialog.Show();
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="parent">The parent window.</param>
        /// <returns>The user selection.</returns>
        public DialogResult Show(Window parent)
        {
            return NativeDialog.Show(parent?.NativeWindow);
        }
    }
}

using System.Collections.Generic;

namespace SpiderEye
{
    /// <summary>
    /// Represents a file dialog.
    /// </summary>
    public abstract class FileDialog : Dialog
    {
        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        public string InitialDirectory
        {
            get { return NativeFileDialog.InitialDirectory; }
            set { NativeFileDialog.InitialDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the selected file name.
        /// </summary>
        public string FileName
        {
            get { return NativeFileDialog.FileName; }
            set { NativeFileDialog.FileName = value; }
        }

        /// <summary>
        /// Gets a collection for adding or removing file filters.
        /// </summary>
        public ICollection<FileFilter> FileFilters
        {
            get { return NativeFileDialog.FileFilters; }
        }

        /// <summary>
        /// Gets the native file dialog.
        /// </summary>
        internal abstract IFileDialog NativeFileDialog { get; }

        /// <inheritdoc/>
        internal override IDialog NativeDialog
        {
            get { return NativeFileDialog; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDialog"/> class.
        /// </summary>
        private protected FileDialog()
        {
        }
    }
}

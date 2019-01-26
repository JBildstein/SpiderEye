using System.Collections.Generic;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a file dialog.
    /// </summary>
    public interface IFileDialog : IDialog
    {
        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the selected file name.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Gets a collection for adding or removing file filters.
        /// </summary>
        ICollection<FileFilter> FileFilters { get; }
    }
}

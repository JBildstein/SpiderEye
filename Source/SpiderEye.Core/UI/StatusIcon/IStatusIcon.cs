using System;

namespace SpiderEye
{
    /// <summary>
    /// Represents a status icon.
    /// </summary>
    internal interface IStatusIcon : IDisposable
    {
        /// <summary>
        /// Gets or sets the status icon title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        AppIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the icon menu.
        /// </summary>
        Menu Menu { get; set; }
    }
}

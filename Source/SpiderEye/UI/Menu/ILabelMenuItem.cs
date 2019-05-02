using System;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents an item in a menu with a label.
    /// </summary>
    public interface ILabelMenuItem : IMenuItem
    {
        /// <summary>
        /// Fires when the menu item is clicked on or otherwise activated.
        /// </summary>
        event EventHandler Click;

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is enabled or not.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the label of this menu item.
        /// </summary>
        string Label { get; set; }
    }
}

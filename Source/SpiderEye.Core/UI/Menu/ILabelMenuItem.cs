using System;

namespace SpiderEye
{
    /// <summary>
    /// Represents an item in a menu with a label.
    /// </summary>
    internal interface ILabelMenuItem : IMenuItem
    {
        /// <summary>
        /// Gets or sets the label of this menu.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Fires when the menu item is clicked on or otherwise activated.
        /// </summary>
        event EventHandler Click;

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is enabled or not.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Sets a keyboard shortcut to the menu item.
        /// </summary>
        /// <param name="modifier">The shortcut modifier key.</param>
        /// <param name="key">The shortcut key.</param>
        void SetShortcut(ModifierKey modifier, Key key);
    }
}

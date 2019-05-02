using System;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents an item in a menu.
    /// </summary>
    public interface IMenuItem
    {
        /// <summary>
        /// /// Adds a new label menu item and returns it.
        /// </summary>
        /// <param name="label">The label text.</param>
        /// /// <returns>The created menu item.</returns>
        ILabelMenuItem AddLabelMenuItem(string label);

        /// <summary>
        /// Adds a new separator menu item.
        /// </summary>
        void AddSeparatorMenuItem();
    }
}

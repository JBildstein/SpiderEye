using System;

namespace SpiderEye
{
    /// <summary>
    /// Represents an item in a menu.
    /// </summary>
    internal interface IMenuItem : IDisposable
    {
        /// <summary>
        /// Creates a new sub-menu for this item.
        /// </summary>
        /// <returns>The created sub-menu.</returns>
        IMenu CreateSubMenu();
    }
}

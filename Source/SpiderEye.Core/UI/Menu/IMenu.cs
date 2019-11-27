using System;

namespace SpiderEye
{
    /// <summary>
    /// Represents a menu.
    /// </summary>
    internal interface IMenu : IDisposable
    {
        /// <summary>
        /// Adds a new menu item to this menu.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddItem(IMenuItem item);
    }
}

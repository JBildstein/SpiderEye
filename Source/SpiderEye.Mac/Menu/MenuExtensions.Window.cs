namespace SpiderEye.Mac
{
    /// <content>
    /// Window menu extensions.
    /// </content>
    public static partial class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item for the window minimize action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddMinimizeItem(this MenuItemCollection menuItems, string label = "Minimize")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performMiniaturize:", ModifierKey.Super, Key.M);
        }

        /// <summary>
        /// Adds a menu item for the window zoom action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddZoomItem(this MenuItemCollection menuItems, string label = "Zoom")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performZoom:");
        }

        /// <summary>
        /// Adds a menu item for the bring windows to front action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddBringAllToFrontItem(this MenuItemCollection menuItems, string label = "Bring All to Front")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "arrangeInFront:");
        }
    }
}

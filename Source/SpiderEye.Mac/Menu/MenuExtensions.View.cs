namespace SpiderEye.Mac
{
    /// <content>
    /// View menu extensions.
    /// </content>
    public static partial class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item for the show toolbar toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddShowToolbarItem(this MenuItemCollection menuItems, string label = "Show Toolbar")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleToolbarShown:", ModifierKey.Super | ModifierKey.Alt, Key.T);
        }

        /// <summary>
        /// Adds a menu item for the toolbar customization action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCustomizeToolbarItem(this MenuItemCollection menuItems, string label = "Customize Toolbar…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "runToolbarCustomizationPalette:");
        }

        /// <summary>
        /// Adds a menu item for the show sidebar toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddShowSidebarItem(this MenuItemCollection menuItems, string label = "Show Sidebar")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleSourceList:", ModifierKey.Super | ModifierKey.Control, Key.S);
        }

        /// <summary>
        /// Adds a menu item for the fullscreen toggle action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddEnterFullScreenItem(this MenuItemCollection menuItems, string label = "Enter Full Screen")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "toggleFullScreen:", ModifierKey.Super | ModifierKey.Control, Key.F);
        }
    }
}

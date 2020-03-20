namespace SpiderEye.Mac
{
    /// <content>
    /// Help menu extensions.
    /// </content>
    public static partial class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item for the show help action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddHelpItem(this MenuItemCollection menuItems, string label = "Help")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "showHelp:");
        }
    }
}

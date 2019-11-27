namespace SpiderEye
{
    /// <summary>
    /// Represents an item in a menu.
    /// </summary>
    public abstract class MenuItem
    {
        /// <summary>
        /// Gets the sub-menu items.
        /// </summary>
        public MenuItemCollection MenuItems { get; }

        /// <summary>
        /// Gets the native menu item.
        /// </summary>
        internal IMenuItem NativeMenuItem { get; }

        private protected MenuItem(IMenuItem menuItem)
        {
            NativeMenuItem = menuItem;
            MenuItems = new MenuItemCollection(menuItem.CreateSubMenu());
        }
    }
}

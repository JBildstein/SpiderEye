using System;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    /// <summary>
    /// Contains extension methods for common macOS menu items.
    /// </summary>
    public static class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item that opens the default about panel.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddAboutItem(this MenuItemCollection menuItems, string label = "About")
        {
            return AddAppMenuItem(menuItems, label, "orderFrontStandardAboutPanel:");
        }

        /// <summary>
        /// Adds a menu item that hides the application.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddHideItem(this MenuItemCollection menuItems, string label = "Hide")
        {
            return AddAppMenuItem(menuItems, label, "hide:", ModifierKey.Super, Key.H);
        }

        /// <summary>
        /// Adds a menu item that hides other applications.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddHideOthersItem(this MenuItemCollection menuItems, string label = "Hide Others")
        {
            return AddAppMenuItem(menuItems, label, "hideOtherApplications:", ModifierKey.Super | ModifierKey.Alt, Key.H);
        }

        /// <summary>
        /// Adds a menu item that shows other applications.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddShowAllItem(this MenuItemCollection menuItems, string label = "Show All")
        {
            return AddAppMenuItem(menuItems, label, "unhideAllApplications:");
        }

        /// <summary>
        /// Adds a menu item that quits the applications.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddQuitItem(this MenuItemCollection menuItems, string label = "Quit")
        {
            if (label == null) { throw new ArgumentNullException(nameof(label)); }

            var item = menuItems.AddLabelItem(label);
            item.SetShortcut(ModifierKey.Super, Key.Q);
            item.Click += (s, e) => Application.Exit();

            return item;
        }

        private static LabelMenuItem AddAppMenuItem(MenuItemCollection menuItems, string label, string command)
        {
            if (menuItems == null) { throw new ArgumentNullException(nameof(menuItems)); }
            if (label == null) { throw new ArgumentNullException(nameof(label)); }

            var item = menuItems.AddLabelItem(label);
            item.Click += (s, e) => ObjC.Call(MacApplication.Handle, command, MacApplication.Handle);

            return item;
        }

        private static LabelMenuItem AddAppMenuItem(MenuItemCollection menuItems, string label, string command, ModifierKey modifier, Key key)
        {
            var item = AddAppMenuItem(menuItems, label, command);
            item.SetShortcut(modifier, key);
            return item;
        }
    }
}

using System;
using AppKit;

namespace SpiderEye.Mac
{
    /// <content>
    /// App menu extensions.
    /// </content>
    public static partial class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item for the open default about panel action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddAboutItem(this MenuItemCollection menuItems, string label = "About")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "orderFrontStandardAboutPanel:");
        }

        /// <summary>
        /// Adds the services menu.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddServicesMenu(this MenuItemCollection menuItems, string label = "Services")
        {
            var nativeMenu = new CocoaLabelMenuItem(label);
            var submenu = nativeMenu.SetSubMenu("services");
            var menu = new LabelMenuItem(nativeMenu);
            menuItems.Add(menu);

            NSApplication.SharedApplication.ServicesMenu = submenu;

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the hide application action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddHideItem(this MenuItemCollection menuItems, string label = "Hide")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "hide:", ModifierKey.Super, Key.H);
        }

        /// <summary>
        /// Adds a menu item for the hide other applications action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddHideOthersItem(this MenuItemCollection menuItems, string label = "Hide Others")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "hideOtherApplications:", ModifierKey.Super | ModifierKey.Alt, Key.H);
        }

        /// <summary>
        /// Adds a menu item for the show other applications action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddShowAllItem(this MenuItemCollection menuItems, string label = "Show All")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "unhideAllApplications:");
        }

        /// <summary>
        /// Adds a menu item for the quit applications action.
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
    }
}

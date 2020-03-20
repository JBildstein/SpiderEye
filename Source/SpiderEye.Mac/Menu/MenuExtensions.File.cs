namespace SpiderEye.Mac
{
    /// <content>
    /// File menu extensions.
    /// </content>
    public static partial class MenuExtensions
    {
        /// <summary>
        /// Adds a menu item for the create new document action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddNewDocumentItem(this MenuItemCollection menuItems, string label = "New")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "newDocument:", ModifierKey.Super, Key.N);
        }

        /// <summary>
        /// Adds a menu item for the open document action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddOpenDocumentItem(this MenuItemCollection menuItems, string label = "Open…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "openDocument:", ModifierKey.Super, Key.O);
        }

        /// <summary>
        /// Adds a menu for the recent document actions.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddRecentDocumentsMenu(this MenuItemCollection menuItems, string label = "Open Recent")
        {
            var menu = menuItems.AddLabelItem(label);
            menu.MenuItems.AddClearRecentDocumentsItem();

            return menu;
        }

        /// <summary>
        /// Adds a menu item for the clear recent documents action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddClearRecentDocumentsItem(this MenuItemCollection menuItems, string label = "Clear Menu")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "clearRecentDocuments:");
        }

        /// <summary>
        /// Adds a menu item for the close document action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddCloseDocumentItem(this MenuItemCollection menuItems, string label = "Close")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "performClose:", ModifierKey.Super, Key.W);
        }

        /// <summary>
        /// Adds a menu item for the save document action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSaveDocumentItem(this MenuItemCollection menuItems, string label = "Save…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "saveDocument:", ModifierKey.Super, Key.S);
        }

        /// <summary>
        /// Adds a menu item for the save document as action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddSaveAsDocumentItem(this MenuItemCollection menuItems, string label = "Save As…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "saveDocumentAs:", ModifierKey.Super | ModifierKey.Shift, Key.S);
        }

        /// <summary>
        /// Adds a menu item for the revert document to last saved verison action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddRevertToSavedDocumentItem(this MenuItemCollection menuItems, string label = "Revert to Saved")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "revertDocumentToSaved:", ModifierKey.Super, Key.R);
        }

        /// <summary>
        /// Adds a menu item for the open page setup dialog action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddPageSetupItem(this MenuItemCollection menuItems, string label = "Page Setup…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "runPageLayout:", ModifierKey.Super | ModifierKey.Shift, Key.P);
        }

        /// <summary>
        /// Adds a menu item for the open print dialog action.
        /// </summary>
        /// <param name="menuItems">The menu item collection to add this menu item to.</param>
        /// <param name="label">The label for the menu item.</param>
        /// <returns>The created menu item.</returns>
        public static MenuItem AddPrintItem(this MenuItemCollection menuItems, string label = "Print…")
        {
            return AddDefaultHandlerMenuItem(menuItems, label, "print:", ModifierKey.Super, Key.P);
        }
    }
}

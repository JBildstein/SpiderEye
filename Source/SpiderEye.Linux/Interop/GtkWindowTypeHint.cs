namespace SpiderEye.Linux.Interop
{
    /// <summary>
    /// These are hints for the window manager that indicate what type of function the window has.
    /// </summary>
    internal enum GdkWindowTypeHint
    {
        /// <summary>
        /// Normal toplevel window.
        /// </summary>
        Normal,

        /// <summary>
        /// Dialog window.
        /// </summary>
        Dialog,

        /// <summary>
        /// Window used to implement a menu; GTK+ uses this hint only for torn-off menus.
        /// </summary>
        Menu,

        /// <summary>
        /// Window used to implement toolbars.
        /// </summary>
        Toolbar,

        /// <summary>
        /// Window used to display a splash screen during application startup.
        /// </summary>
        Splashscreen,

        /// <summary>
        /// Utility windows which are not detached toolbars or dialogs.
        /// </summary>
        Utility,

        /// <summary>
        /// Used for creating dock or panel windows.
        /// </summary>
        Dock,

        /// <summary>
        /// Used for creating the desktop background window.
        /// </summary>
        Desktop,

        /// <summary>
        /// A menu that belongs to a menubar.
        /// </summary>
        DropdownMenu,

        /// <summary>
        /// A menu that does not belong to a menubar, e.g.a context menu.
        /// </summary>
        PopupMenu,

        /// <summary>
        /// A tooltip.
        /// </summary>
        Tooltip,

        /// <summary>
        /// A notification - typically a "bubble" that belongs to a status icon.
        /// </summary>
        Notification,

        /// <summary>
        /// A popup from a combo box.
        /// </summary>
        Combo,

        /// <summary>
        ///  window that is used to implement a DND cursor.
        /// </summary>
        Dnd,
    }
}

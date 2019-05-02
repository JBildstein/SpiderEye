using System;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux.Menu
{
    internal class GtkMenu : GtkMenuItem, IMenu
    {
        public GtkMenu()
            : base(Gtk.Menu.Create())
        {
        }

        protected override void AddItem(IntPtr item)
        {
            Gtk.Menu.AddItem(Handle, item);
        }
    }
}

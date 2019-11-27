using System;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkMenuItem : IMenuItem
    {
        public readonly IntPtr Handle;

        protected GtkMenuItem(IntPtr handle)
        {
            Handle = handle;
        }

        public IMenu CreateSubMenu()
        {
            return new GtkSubMenu(Handle);
        }

        public void Dispose()
        {
            Gtk.Widget.Destroy(Handle);
        }

        private sealed class GtkSubMenu : IMenu
        {
            private readonly IntPtr menuItem;
            private GtkMenu menu;

            public GtkSubMenu(IntPtr menuItem)
            {
                this.menuItem = menuItem;
            }

            public void AddItem(IMenuItem item)
            {
                if (item == null) { throw new ArgumentNullException(nameof(item)); }

                if (menu == null)
                {
                    menu = new GtkMenu();
                    Gtk.Menu.SetSubmenu(menuItem, menu.Handle);
                }

                menu.AddItem(item);
            }

            public void Dispose()
            {
                menu.Dispose();
            }
        }
    }
}

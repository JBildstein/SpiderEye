using System;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal abstract class CocoaMenuItem : IMenuItem
    {
        public readonly IntPtr Handle;

        protected CocoaMenuItem(IntPtr handle)
        {
            Handle = handle;
        }

        public IMenu CreateSubMenu()
        {
            return new CocoaSubMenu(Handle);
        }

        public void Dispose()
        {
            // don't think anything needs to be done here
        }

        private sealed class CocoaSubMenu : IMenu
        {
            private readonly IntPtr menuItem;
            private CocoaMenu menu;

            public CocoaSubMenu(IntPtr menuItem)
            {
                this.menuItem = menuItem;
            }

            public void AddItem(IMenuItem item)
            {
                if (item == null) { throw new ArgumentNullException(nameof(item)); }

                if (menu == null)
                {
                    menu = new CocoaMenu();
                    ObjC.Call(menuItem, "setSubmenu:", menu.Handle);
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

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

        public virtual IMenu CreateSubMenu()
        {
            return new CocoaSubMenu(Handle);
        }

        public void Dispose()
        {
            // don't think anything needs to be done here
        }

        protected sealed class CocoaSubMenu : IMenu
        {
            public string Title
            {
                get
                {
                    if (menu == null) { return title; }
                    else { return menu.Title; }
                }
                set
                {
                    if (menu == null) { title = value; }
                    else { menu.Title = value; }
                }
            }

            public CocoaMenu NativeMenu
            {
                get { return menu; }
            }

            private readonly IntPtr menuItem;
            private string title;
            private CocoaMenu menu;

            public CocoaSubMenu(IntPtr menuItem)
                : this(menuItem, null)
            {
            }

            public CocoaSubMenu(IntPtr menuItem, string title)
                : this(menuItem, title, false)
            {
            }

            public CocoaSubMenu(IntPtr menuItem, string title, bool createImmediately)
            {
                this.menuItem = menuItem;
                this.title = title;
                if (createImmediately) { SetNativeMenu(); }
            }

            public void AddItem(IMenuItem item)
            {
                if (item == null) { throw new ArgumentNullException(nameof(item)); }

                if (menu == null) { SetNativeMenu(); }

                menu.AddItem(item);
            }

            public void Dispose()
            {
                menu.Dispose();
            }

            private void SetNativeMenu()
            {
                menu = new CocoaMenu(title);
                ObjC.Call(menuItem, "setSubmenu:", menu.Handle);
            }
        }
    }
}

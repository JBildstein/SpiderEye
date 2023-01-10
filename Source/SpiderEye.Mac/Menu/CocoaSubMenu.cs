using System;
using System.Diagnostics.CodeAnalysis;
using AppKit;

namespace SpiderEye.Mac
{
    internal sealed class CocoaSubMenu : IMenu
    {
        public string? Title
        {
            get
            {
                if (menu == null) { return title; }
                else { return menu.Title; }
            }
            set
            {
                if (menu == null) { title = value; }
                else { menu.Title = value ?? string.Empty; }
            }
        }

        public CocoaMenu? NativeMenu
        {
            get { return menu; }
        }

        private readonly NSMenuItem menuItem;
        private string? title;
        private CocoaMenu? menu;

        public CocoaSubMenu(NSMenuItem menuItem)
            : this(menuItem, null)
        {
        }

        public CocoaSubMenu(NSMenuItem menuItem, string? title)
            : this(menuItem, title, false)
        {
        }

        public CocoaSubMenu(NSMenuItem menuItem, string? title, bool createImmediately)
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
            menu?.Dispose();
        }

        [MemberNotNull(nameof(menu))]
        private void SetNativeMenu()
        {
            menu = new CocoaMenu(title);
            menuItem.Submenu = menu;
        }
    }
}

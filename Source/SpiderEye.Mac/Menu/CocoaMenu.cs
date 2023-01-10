using System;
using AppKit;

namespace SpiderEye.Mac
{
    internal class CocoaMenu : NSMenu, IMenu
    {
        public CocoaMenu()
            : this(null)
        {
        }

        public CocoaMenu(string? title)
            : base(title ?? string.Empty)
        {
        }

        public void AddItem(IMenuItem item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            AddItem((NSMenuItem)item);
        }
    }
}

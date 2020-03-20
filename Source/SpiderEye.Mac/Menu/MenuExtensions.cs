using System;

namespace SpiderEye.Mac
{
    /// <summary>
    /// Contains extension methods for common macOS menu items.
    /// </summary>
    public static partial class MenuExtensions
    {
        private static LabelMenuItem AddDefaultHandlerMenuItem(MenuItemCollection menuItems, string label, string command, long tag = 0)
        {
            if (menuItems == null) { throw new ArgumentNullException(nameof(menuItems)); }
            if (label == null) { throw new ArgumentNullException(nameof(label)); }

            // when setting target to null, cocoa will look for the first responder that can handle the action
            var nativeItem = new CocoaLabelMenuItem(label, command, null, 0);
            var item = new LabelMenuItem(nativeItem);

            menuItems.Add(item);

            return item;
        }

        private static LabelMenuItem AddDefaultHandlerMenuItem(MenuItemCollection menuItems, string label, string command, ModifierKey modifier, Key key, long tag = 0)
        {
            var item = AddDefaultHandlerMenuItem(menuItems, label, command, tag);
            item.SetShortcut(modifier, key);
            return item;
        }
    }
}

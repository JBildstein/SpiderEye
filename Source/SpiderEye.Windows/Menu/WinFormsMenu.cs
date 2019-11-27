using System;
using System.Windows.Forms;
using SpiderEye.Tools;

namespace SpiderEye.Windows
{
    internal class WinFormsMenu : IMenu
    {
        public readonly ContextMenu Menu = new ContextMenu();

        public void AddItem(IMenuItem item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            var nativeItem = NativeCast.To<WinFormsMenuItem>(item);
            Menu.MenuItems.Add(nativeItem.Item);
        }

        public void Dispose()
        {
            Menu.Dispose();
        }
    }
}

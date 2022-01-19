using System;
using System.Windows.Forms;
using SpiderEye.Tools;

namespace SpiderEye.Windows
{
    internal class WinFormsMenu : IMenu
    {
        public readonly ContextMenuStrip Menu = new();

        public void AddItem(IMenuItem item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            var nativeItem = NativeCast.To<WinFormsMenuItem>(item);
            Menu.Items.Add(nativeItem.Item);
        }

        public void Dispose()
        {
            Menu.Dispose();
        }
    }
}

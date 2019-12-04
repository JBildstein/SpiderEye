using System;
using System.Windows.Forms;
using SpiderEye.Tools;

namespace SpiderEye.Windows
{
    internal abstract class WinFormsMenuItem : IMenuItem
    {
        public abstract ToolStripItem Item { get; }

        public IMenu CreateSubMenu()
        {
            return new WinFormsSubMenu(Item);
        }

        public void Dispose()
        {
            Item.Dispose();
        }

        private sealed class WinFormsSubMenu : IMenu
        {
            private readonly ToolStripMenuItem menuItem;
            private readonly bool canAdd;

            public WinFormsSubMenu(ToolStripItem menuItem)
            {
                this.menuItem = menuItem as ToolStripMenuItem;
                canAdd = this.menuItem != null;
            }

            public void AddItem(IMenuItem item)
            {
                if (item == null) { throw new ArgumentNullException(nameof(item)); }
                if (!canAdd) { throw new InvalidOperationException("This menu item cannot have sub-items."); }

                var nativeItem = NativeCast.To<WinFormsMenuItem>(item);
                menuItem.DropDownItems.Add(nativeItem.Item);
            }

            public void Dispose()
            {
                // nothing to do here, managed by WinFormsMenuItem class
            }
        }
    }
}

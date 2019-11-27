using System;
using SpiderEye.Tools;
using WFMenuItem = System.Windows.Forms.MenuItem;

namespace SpiderEye.Windows
{
    internal abstract class WinFormsMenuItem : IMenuItem
    {
        public abstract WFMenuItem Item { get; }

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
            private readonly WFMenuItem menuItem;

            public WinFormsSubMenu(WFMenuItem menuItem)
            {
                this.menuItem = menuItem;
            }

            public void AddItem(IMenuItem item)
            {
                if (item == null) { throw new ArgumentNullException(nameof(item)); }

                var nativeItem = NativeCast.To<WinFormsMenuItem>(item);
                menuItem.MenuItems.Add(nativeItem.Item);
            }

            public void Dispose()
            {
                // nothing to do here, managed by WinFormsMenuItem class
            }
        }
    }
}

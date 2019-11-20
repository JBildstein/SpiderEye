using System.Windows.Forms;

namespace SpiderEye.UI.Windows.Menu
{
    internal class WinFormsMenu : WinFormsMenuItem, IMenu
    {
        public readonly ContextMenu Menu = new ContextMenu();

        public void Dispose()
        {
            Menu.Dispose();
        }

        protected override void AddItem(MenuItem item)
        {
            Menu.MenuItems.Add(item);
        }

        protected override void SetShortcut(Shortcut shortcut)
        {
            // ignore: no shortcuts for the base menu class
        }
    }
}

using System;
using System.Windows.Forms;

namespace SpiderEye.UI.Windows.Menu
{
    internal class WinFormsLabelMenuItem : WinFormsMenuItem, ILabelMenuItem
    {
        public event EventHandler Click;

        public bool Enabled
        {
            get { return Item.Enabled; }
            set { Item.Enabled = value; }
        }

        public string Label
        {
            get { return Item.Text; }
            set { Item.Text = value; }
        }

        public readonly MenuItem Item;

        public WinFormsLabelMenuItem(string label)
        {
            Item = new MenuItem(label);
            Item.Click += (s, e) => Click?.Invoke(this, EventArgs.Empty);
        }

        protected override void AddItem(MenuItem item)
        {
            Item.MenuItems.Add(item);
        }

        protected override void SetShortcut(Shortcut shortcut)
        {
            Item.Shortcut = shortcut;
        }
    }
}

using System;
using SpiderEye.Windows.Interop;

namespace SpiderEye.Windows
{
    internal class WinFormsLabelMenuItem : WinFormsMenuItem, ILabelMenuItem
    {
        public event EventHandler Click
        {
            add { Item.Click += value; }
            remove { Item.Click -= value; }
        }

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

        public override System.Windows.Forms.MenuItem Item { get; }

        public WinFormsLabelMenuItem(string label)
        {
            Item = new System.Windows.Forms.MenuItem(label);
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            var shortcut = KeyMapper.GetShortcut(modifier, key);
            Item.Shortcut = shortcut;
        }
    }
}

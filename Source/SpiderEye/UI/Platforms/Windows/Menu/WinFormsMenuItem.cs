using System.Windows.Forms;
using SpiderEye.UI.Platforms.Windows.Interop;

namespace SpiderEye.UI.Windows.Menu
{
    internal abstract class WinFormsMenuItem : IMenuItem
    {
        public ILabelMenuItem AddLabelMenuItem(string label)
        {
            var item = new WinFormsLabelMenuItem(label);
            AddItem(item.Item);
            return item;
        }

        public void AddSeparatorMenuItem()
        {
            AddItem(new MenuItem("-"));
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            SetShortcut(KeyMapper.GetShortcut(modifier, key));
        }

        protected abstract void AddItem(MenuItem item);

        protected abstract void SetShortcut(Shortcut shortcut);
    }
}

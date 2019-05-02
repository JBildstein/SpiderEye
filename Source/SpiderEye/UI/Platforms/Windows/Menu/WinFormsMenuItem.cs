using System.Windows.Forms;

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

        protected abstract void AddItem(MenuItem item);
    }
}

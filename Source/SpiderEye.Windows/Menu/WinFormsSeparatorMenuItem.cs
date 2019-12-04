using System.Windows.Forms;

namespace SpiderEye.Windows
{
    internal class WinFormsSeparatorMenuItem : WinFormsMenuItem
    {
        public override ToolStripItem Item { get; }

        public WinFormsSeparatorMenuItem()
        {
            Item = new ToolStripSeparator();
        }
    }
}

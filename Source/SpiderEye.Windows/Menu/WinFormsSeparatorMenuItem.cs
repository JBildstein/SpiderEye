namespace SpiderEye.Windows
{
    internal class WinFormsSeparatorMenuItem : WinFormsMenuItem
    {
        public override System.Windows.Forms.MenuItem Item { get; }

        public WinFormsSeparatorMenuItem()
        {
            Item = new System.Windows.Forms.MenuItem("-");
        }
    }
}

using AppKit;

namespace SpiderEye.Mac
{
    internal class CocoaSeparatorMenuItem : NSMenuItem, IMenuItem
    {
        public CocoaSeparatorMenuItem()
            : base(NSMenuItem.SeparatorItem.Handle)
        {
        }

        public IMenu CreateSubMenu()
        {
            return new CocoaSubMenu(this);
        }
    }
}

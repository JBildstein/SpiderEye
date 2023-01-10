using AppKit;

namespace SpiderEye.Mac
{
    internal abstract class CocoaMenuItem : NSMenuItem, IMenuItem
    {
        public virtual IMenu CreateSubMenu()
        {
            return new CocoaSubMenu(this);
        }
    }
}

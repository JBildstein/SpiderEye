using System;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Menu
{
    internal abstract class CocoaMenuItem : IMenuItem
    {
        public readonly IntPtr Handle;

        protected CocoaMenuItem(IntPtr handle)
        {
            Handle = handle;
        }

        public ILabelMenuItem AddLabelMenuItem(string label)
        {
            var item = new CocoaLabelMenuItem(label);
            AddItem(item.Handle);

            return item;
        }

        public void AddSeparatorMenuItem()
        {
            var item = AppKit.Call("NSMenuItem", "separatorItem");
            AddItem(item);
        }

        public void Dispose()
        {
            // don't think anything needs to be done here
        }

        internal protected abstract void AddItem(IntPtr item);
    }
}

using System;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaMenu : IMenu
    {
        public readonly IntPtr Handle;

        public CocoaMenu()
        {
            Handle = AppKit.Call("NSMenu", "new");
            ObjC.Call(Handle, "setAutoenablesItems:", false);
        }

        public void AddItem(IMenuItem item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }

            var nativeItem = NativeCast.To<CocoaMenuItem>(item);
            ObjC.Call(Handle, "addItem:", nativeItem.Handle);
        }

        public void Dispose()
        {
            // don't think anything needs to be done here
        }
    }
}

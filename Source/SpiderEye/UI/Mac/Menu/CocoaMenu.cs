using System;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Menu
{
    internal class CocoaMenu : CocoaMenuItem, IMenu
    {
        public CocoaMenu()
            : base(AppKit.Call("NSMenu", "new"))
        {
            ObjC.Call(Handle, "setAutoenablesItems:", false);
        }

        protected internal override void AddItem(IntPtr item)
        {
            ObjC.Call(Handle, "addItem:", item);
        }
    }
}

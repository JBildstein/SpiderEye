using System;
using SpiderEye.UI.Mac.Native;
using SpiderEye.UI.Platforms.Mac.Interop;

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

        protected override void SetShortcut(NSEventModifierFlags modifier, string key)
        {
            // ignore: no shortcuts for the base menu class
        }
    }
}

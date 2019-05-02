using System;
using System.Threading;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;
using SpiderEye.UI.Platforms.Mac.Interop;

namespace SpiderEye.UI.Mac.Menu
{
    internal class CocoaLabelMenuItem : CocoaMenuItem, ILabelMenuItem
    {
        public event EventHandler Click;

        private static int classCount;
        private CocoaMenu submenu;

        public string Label
        {
            get { return NSString.GetString(ObjC.Call(Handle, "title")); }
            set { ObjC.Call(Handle, "setTitle:", NSString.Create(value)); }
        }

        public bool Enabled
        {
            get { return ObjC.Call(Handle, "enabled") != IntPtr.Zero; }
            set { ObjC.Call(Handle, "setEnabled:", value); }
        }

        public CocoaLabelMenuItem(string label)
            : base(AppKit.Call("NSMenuItem", "alloc"))
        {
            ObjC.Call(
                Handle,
                "initWithTitle:action:keyEquivalent:",
                NSString.Create(label),
                ObjC.RegisterName("menuCallback:"),
                NSString.Create(string.Empty));

            SetCallbackClass();
        }

        protected internal override void AddItem(IntPtr item)
        {
            if (submenu == null)
            {
                submenu = new CocoaMenu();
                ObjC.Call(Handle, "setSubmenu:", submenu.Handle);
            }

            submenu.AddItem(item);
        }

        protected override void SetShortcut(NSEventModifierFlags modifier, string key)
        {
            ObjC.Call(Handle, "keyEquivalentModifierMask", (uint)modifier);
            ObjC.Call(Handle, "keyEquivalent", NSString.Create(key));
        }

        private void SetCallbackClass()
        {
            string name = "MenuCallbackObject" + Interlocked.Increment(ref classCount);
            IntPtr callbackClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), name, IntPtr.Zero);

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("menuCallback:"),
                (MenuCallbackDelegate)MenuCallback,
                "v@:@");

            ObjC.RegisterClassPair(callbackClass);
            ObjC.Call(Handle, "setTarget:", ObjC.Call(callbackClass, "new"));
        }

        private void MenuCallback(IntPtr self, IntPtr op, IntPtr menu)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}

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
            get { return NSString.GetString(GetTitle()); }
            set
            {
                IntPtr title = NSString.Create(value);
                SetTitle(Handle, title);
                if (submenu != null) { SetTitle(submenu.Handle, title); }
            }
        }

        public bool Enabled
        {
            get { return ObjC.Call(Handle, "enabled") != IntPtr.Zero; }
            set { ObjC.Call(Handle, "setEnabled:", value); }
        }

        private readonly MenuCallbackDelegate menuDelegate;

        public CocoaLabelMenuItem(string label)
            : base(AppKit.Call("NSMenuItem", "alloc"))
        {
            // need to keep the delegate around or it will get garbage collected
            menuDelegate = MenuCallback;

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
                SetTitle(submenu.Handle, GetTitle());
                ObjC.Call(Handle, "setSubmenu:", submenu.Handle);
            }

            submenu.AddItem(item);
        }

        protected override void SetShortcut(NSEventModifierFlags modifier, string key)
        {
            ObjC.Call(Handle, "setKeyEquivalentModifierMask:", new UIntPtr((ulong)modifier));
            ObjC.Call(Handle, "setKeyEquivalent:", NSString.Create(key));
        }

        private IntPtr GetTitle()
        {
            return ObjC.Call(Handle, "title");
        }

        private void SetTitle(IntPtr handle, IntPtr value)
        {
            ObjC.Call(handle, "setTitle:", value);
        }

        private void SetCallbackClass()
        {
            string name = "MenuCallbackObject" + Interlocked.Increment(ref classCount);
            IntPtr callbackClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), name, IntPtr.Zero);

            ObjC.AddMethod(
                callbackClass,
                ObjC.RegisterName("menuCallback:"),
                menuDelegate,
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

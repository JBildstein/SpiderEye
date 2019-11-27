using System;
using System.Threading;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaLabelMenuItem : CocoaMenuItem, ILabelMenuItem
    {
        public event EventHandler Click;

        private static int classCount;

        public string Label
        {
            get { return NSString.GetString(GetTitle()); }
            set
            {
                IntPtr title = NSString.Create(value);
                SetTitle(Handle, title);
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

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            NSEventModifierFlags nsModifier = KeyMapper.GetModifier(modifier);
            string mappedKey = KeyMapper.GetKey(key);

            ObjC.Call(Handle, "setKeyEquivalentModifierMask:", new UIntPtr((ulong)nsModifier));
            ObjC.Call(Handle, "setKeyEquivalent:", NSString.Create(mappedKey));
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

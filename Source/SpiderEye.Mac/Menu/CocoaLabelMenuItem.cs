using System;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaLabelMenuItem : CocoaMenuItem, ILabelMenuItem
    {
        public event EventHandler Click;

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

        private static readonly NativeClassDefinition CallbackClassDefinition;

        private readonly NativeClassInstance callbackClass;

        static CocoaLabelMenuItem()
        {
            CallbackClassDefinition = CreateCallbackClass();
        }

        public CocoaLabelMenuItem(string label)
            : base(AppKit.Call("NSMenuItem", "alloc"))
        {
            callbackClass = CallbackClassDefinition.CreateInstance(this);

            ObjC.Call(
                Handle,
                "initWithTitle:action:keyEquivalent:",
                NSString.Create(label),
                ObjC.RegisterName("menuCallback:"),
                NSString.Create(string.Empty));

            ObjC.Call(Handle, "setTarget:", callbackClass.Handle);
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            NSEventModifierFlags nsModifier = KeyMapper.GetModifier(modifier);
            string mappedKey = KeyMapper.GetKey(key);

            ObjC.Call(Handle, "setKeyEquivalentModifierMask:", new UIntPtr((ulong)nsModifier));
            ObjC.Call(Handle, "setKeyEquivalent:", NSString.Create(mappedKey));
        }

        private static NativeClassDefinition CreateCallbackClass()
        {
            var definition = new NativeClassDefinition("SpiderEyeMenuCallback");

            definition.AddMethod<MenuCallbackDelegate>(
                "menuCallback:",
                "v@:@",
                (self, op, menu) =>
                {
                    var instance = definition.GetParent<CocoaLabelMenuItem>(self);
                    instance.Click?.Invoke(instance, EventArgs.Empty);
                });

            definition.FinishDeclaration();

            return definition;
        }

        private IntPtr GetTitle()
        {
            return ObjC.Call(Handle, "title");
        }

        private void SetTitle(IntPtr handle, IntPtr value)
        {
            ObjC.Call(handle, "setTitle:", value);
        }
    }
}

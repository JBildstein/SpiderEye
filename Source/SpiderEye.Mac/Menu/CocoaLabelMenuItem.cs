using System;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaLabelMenuItem : CocoaMenuItem, ILabelMenuItem
    {
        public event EventHandler? Click;

        public string? Label
        {
            get
            {
                IntPtr title = ObjC.Call(Handle, "title");
                return NSString.GetString(title);
            }
            set
            {
                IntPtr title = NSString.Create(value);
                ObjC.Call(Handle, "setTitle:", title);
                if (subMenu != null) { subMenu.Title = value; }
            }
        }

        public bool Enabled
        {
            get { return ObjC.Call(Handle, "enabled") != IntPtr.Zero; }
            set { ObjC.Call(Handle, "setEnabled:", value); }
        }

        private static readonly NativeClassDefinition CallbackClassDefinition;

        private readonly NativeClassInstance? callbackClass;
        private CocoaSubMenu? subMenu;

        static CocoaLabelMenuItem()
        {
            CallbackClassDefinition = CreateCallbackClass();
        }

        public CocoaLabelMenuItem(string label)
            : this(label, "menuCallback:")
        {
            callbackClass = CallbackClassDefinition.CreateInstance(this);
            SetTarget(callbackClass.Handle);
        }

        public CocoaLabelMenuItem(string label, string action, string target)
            : this(label, action)
        {
            SetTarget(ObjC.RegisterName(target));
        }

        public CocoaLabelMenuItem(string label, string action, string? target, long tag)
            : this(label, action)
        {
            SetTarget(ObjC.RegisterName(target));
            SetTag(tag);
        }

        private CocoaLabelMenuItem(string label, string action)
            : base(AppKit.Call("NSMenuItem", "alloc"))
        {
            ObjC.Call(
                Handle,
                "initWithTitle:action:keyEquivalent:",
                NSString.Create(label),
                ObjC.RegisterName(action),
                NSString.Create(string.Empty));
        }

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            NSEventModifierFlags nsModifier = KeyMapper.GetModifier(modifier);
            string? mappedKey = KeyMapper.GetKey(key);
            if (mappedKey == null) { return; }

            ObjC.Call(Handle, "setKeyEquivalentModifierMask:", new UIntPtr((ulong)nsModifier));
            ObjC.Call(Handle, "setKeyEquivalent:", NSString.Create(mappedKey));
        }

        public override IMenu CreateSubMenu()
        {
            return subMenu = new CocoaSubMenu(Handle, Label);
        }

        public CocoaMenu SetSubMenu(string label)
        {
            if (label == null) { throw new ArgumentNullException(nameof(label)); }
            if (subMenu != null) { throw new InvalidOperationException("Submenu is already created"); }

            subMenu = new CocoaSubMenu(Handle, label, true);

            return subMenu.NativeMenu!;
        }

        private static NativeClassDefinition CreateCallbackClass()
        {
            var definition = NativeClassDefinition.FromObject("SpiderEyeMenuCallback");

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

        private void SetTarget(IntPtr target)
        {
            ObjC.Call(Handle, "setTarget:", target);
        }

        private void SetTag(long tag)
        {
            ObjC.Call(Handle, "setTag:", new IntPtr(tag));
        }
    }
}

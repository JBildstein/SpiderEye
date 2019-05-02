using System;
using SpiderEye.UI.Mac.Native;
using SpiderEye.UI.Platforms.Mac.Interop;

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

        public void SetShortcut(ModifierKey modifier, Key key)
        {
            SetShortcut(KeyMapper.GetModifier(modifier), KeyMapper.GetKey(key));
        }

        protected internal abstract void AddItem(IntPtr item);

        protected abstract void SetShortcut(NSEventModifierFlags modifier, string key);
    }
}

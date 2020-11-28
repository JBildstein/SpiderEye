using System;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaMenu : IMenu
    {
        public string? Title
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
            }
        }

        public readonly IntPtr Handle;

        public CocoaMenu()
            : this(null)
        {
        }

        public CocoaMenu(string? title)
        {
            if (title != null)
            {
                Handle = AppKit.Call("NSMenu", "alloc");
                ObjC.Call(Handle, "initWithTitle:", NSString.Create(title));
            }
            else { Handle = AppKit.Call("NSMenu", "new"); }
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

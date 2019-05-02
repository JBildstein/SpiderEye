using System;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Interop
{
    internal class NSDialog : IDisposable
    {
        public readonly IntPtr Handle;
        private readonly IntPtr window;
        private bool release;

        private delegate void Callback(IntPtr self, IntPtr result);

        private NSDialog(IntPtr handle, IntPtr window, bool release)
        {
            Handle = handle;
            this.window = window;
            this.release = release;
        }

        public static NSDialog CreateAlert()
        {
            var alert = AppKit.Call("NSAlert", "new");
            return new NSDialog(alert, ObjC.Call(alert, "window"), true);
        }

        public static NSDialog CreateSavePanel()
        {
            var panel = AppKit.Call("NSSavePanel", "savePanel");
            return new NSDialog(panel, panel, false);
        }

        public static NSDialog CreateOpenPanel()
        {
            var panel = AppKit.Call("NSOpenPanel", "openPanel");
            return new NSDialog(panel, panel, false);
        }

        public int Run(CocoaWindow parent)
        {
            if (Handle == IntPtr.Zero) { throw new InvalidOperationException("Dialog is null"); }

            if (parent == null) { return ObjC.Call(Handle, "runModal").ToInt32(); }

            var app = CocoaApplication.GetApp();
            var block = new NSBlock((Callback)((s, r) => ObjC.Call(app, "stopModalWithCode:", r)));
            ObjC.Call(Handle, "beginSheetModalForWindow:completionHandler:", parent.Handle, block.Handle);
            return ObjC.Call(app, "runModalForWindow:", window).ToInt32();
        }

        public void Dispose()
        {
            if (release && Handle != IntPtr.Zero) { ObjC.Call(Handle, "release"); }
        }
    }
}

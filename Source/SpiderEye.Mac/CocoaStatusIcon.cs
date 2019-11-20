using System;
using SpiderEye.UI.Mac.Menu;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaStatusIcon : IStatusIcon
    {
        public string Title
        {
            get; // TODO: see if setting title is useful on macOS StatusBar
            set;
        }

        public AppIcon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                UpdateIcon(value);
            }
        }

        private readonly IntPtr statusItem;
        private readonly IntPtr statusBarButton;

        private AppIcon icon;

        public CocoaStatusIcon()
        {
            var statusBar = AppKit.Call("NSStatusBar", "systemStatusBar");
            statusItem = ObjC.Call(statusBar, "statusItemWithLength:", -2.0); // -1 = variable size; -2 = square size
            ObjC.Call(statusItem, "setHighlightMode:", true);
            statusBarButton = ObjC.Call(statusItem, "button");
            ObjC.Call(statusBarButton, "setImageScaling:", 3); // 3 = scale proportionally up or down
        }

        public IMenu AddMenu()
        {
            var menu = new CocoaMenu();
            ObjC.Call(statusItem, "setMenu:", menu.Handle);

            return menu;
        }

        public void Dispose()
        {
            // don't think anything needs to be done here
        }

        private unsafe void UpdateIcon(AppIcon icon)
        {
            var image = IntPtr.Zero;
            if (icon != null && icon.Icons.Length > 0)
            {
                byte[] data = icon.GetIconData(icon.DefaultIcon);
                fixed (byte* dataPtr = data)
                {
                    IntPtr nsData = Foundation.Call(
                        "NSData",
                        "dataWithBytesNoCopy:length:freeWhenDone:",
                        (IntPtr)dataPtr,
                        new IntPtr(data.Length),
                        IntPtr.Zero);

                    image = AppKit.Call("NSImage", "alloc");
                    ObjC.Call(image, "initWithData:", nsData);
                    ObjC.Call(statusBarButton, "setImage:", image);
                }
            }

            ObjC.Call(statusBarButton, "setImage:", image);
        }
    }
}

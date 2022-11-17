using AppKit;
using Foundation;

namespace SpiderEye.Mac
{
    internal class CocoaStatusIcon : IStatusIcon
    {
        public string? Title
        {
            get { return statusItem.Title; }
            set
            {
                statusItem.Title = value ?? string.Empty;
            }
        }

        public AppIcon? Icon
        {
            get { return icon; }
            set
            {
                icon = value;

                NSImage? image = null;
                if (value != null && value.Icons.Length > 0)
                {
                    byte[] data = value.GetIconData(value.DefaultIcon);
                    using var nsData = NSData.FromArray(data);
                    image = new NSImage(nsData);
                }

                statusItem.Button.Image = image;
            }
        }

        public Menu? Menu
        {
            get { return menu; }
            set
            {
                menu = value;
                statusItem.Menu = (NSMenu?)value?.NativeMenu;
            }
        }

        private readonly NSStatusItem statusItem;

        private AppIcon? icon;
        private Menu? menu;

        public CocoaStatusIcon(string title)
        {
            var statusBar = NSStatusBar.SystemStatusBar;

            statusItem = statusBar.CreateStatusItem(NSStatusItemLength.Square);
            statusItem.HighlightMode = true;
            statusItem.Title = title;
            statusItem.Button.ImageScaling = NSImageScale.ProportionallyUpOrDown;
        }

        public void Dispose()
        {
            statusItem.StatusBar.RemoveStatusItem(statusItem);
            statusItem.Dispose();
        }
    }
}

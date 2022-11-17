using System;
using AppKit;
using CoreGraphics;
using Foundation;
using SpiderEye.Bridge;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    internal class CocoaWindow : NSWindow, IWindow
    {
        public event CancelableEventHandler? Closing;
        public event EventHandler? Closed;
        public event EventHandler? Shown;

        string? IWindow.Title
        {
            get { return this.Title; }
            set { this.Title = value ?? string.Empty; }
        }

        public Size Size
        {
            get
            {
                var frame = this.Frame;
                return new Size((int)frame.Size.Width, (int)frame.Size.Height);
            }
            set
            {
                this.SetContentSize(new CGSize(value.Width, value.Height));
            }
        }

        Size IWindow.MinSize
        {
            get
            {
                var size = this.ContentMinSize;
                return new Size(size.Width, size.Height);
            }
            set
            {
                this.ContentMinSize = new CGSize(value.Width, value.Height);
            }
        }

        Size IWindow.MaxSize
        {
            get
            {
                var size = this.ContentMaxSize;
                return new Size(size.Width, size.Height);
            }
            set
            {
                if (value == Size.Zero) { value = new Size(float.MaxValue, float.MaxValue); }
                this.ContentMaxSize = new CGSize(value.Width, value.Height);
            }
        }

        public bool CanResize
        {
            get { return canResizeField; }
            set
            {
                canResizeField = value;
                StyleMask = GetWantedStyleMask(StyleMask, borderStyleField, value);
            }
        }

        public WindowBorderStyle BorderStyle
        {
            get { return borderStyleField; }
            set
            {
                borderStyleField = value;

                // the title gets reset when setting it to borderless
                // so we just store the title, set the border and set the title back again
                string? title = Title;
                StyleMask = GetWantedStyleMask(StyleMask, value, canResizeField);
                Title = title;
            }
        }

        string? IWindow.BackgroundColor
        {
            get { return backgroundColorField; }
            set
            {
                backgroundColorField = value;

                ColorTools.ParseHex(value, out byte r, out byte g, out byte b);
                using var color = NSColor.FromRgba(r, g, b, (byte)255);
                BackgroundColor = color;

                using var key = new NSString("backgroundColor");
                webview.SetValueForKey(color, key);
            }
        }

        public bool UseBrowserTitle
        {
            get { return webview.UseBrowserTitle; }
            set { webview.UseBrowserTitle = value; }
        }

        // is ignored because there are no window icons on macOS
        public AppIcon? Icon { get; set; }

        public bool EnableScriptInterface
        {
            get { return webview.EnableScriptInterface; }
            set { webview.EnableScriptInterface = value; }
        }

        public bool EnableDevTools
        {
            get { return webview.EnableDevTools; }
            set { webview.EnableDevTools = value; }
        }

        public IWebview Webview
        {
            get { return webview; }
        }

        private readonly CocoaWebview webview;

        private bool canResizeField;
        private WindowBorderStyle borderStyleField;
        private string? backgroundColorField;

        public CocoaWindow(WindowConfiguration config, WebviewBridge bridge)
            : base(new CGRect(0, 0, config.Size.Width, config.Size.Height), GetWantedStyleMask(0ul, WindowBorderStyle.Default, config.CanResize), NSBackingStore.Buffered, false)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (bridge == null) { throw new ArgumentNullException(nameof(bridge)); }

            canResizeField = config.CanResize;
            StyleMask = GetWantedStyleMask(StyleMask, borderStyleField, canResizeField);

            webview = new CocoaWebview(bridge);
            ContentView = webview;

            webview.TitleChanged += Webview_TitleChanged;

            Delegate = new CocoaWindowDelegate(this);
        }

        public void Show()
        {
            Center();
            MakeKeyAndOrderFront(null);

            MacApplication.SynchronizationContext.Post(s => Shown?.Invoke(this, EventArgs.Empty), null);
        }

        public void EnterFullscreen()
        {
            if (!StyleMask.HasFlag(NSWindowStyle.FullScreenWindow))
            {
                ToggleFullScreen(this);
            }
        }

        public void ExitFullscreen()
        {
            if (StyleMask.HasFlag(NSWindowStyle.FullScreenWindow))
            {
                ToggleFullScreen(this);
            }
        }

        public void Maximize()
        {
            if (!IsZoomed)
            {
                Zoom(this);
            }
        }

        public void Unmaximize()
        {
            if (IsZoomed)
            {
                Zoom(this);
            }
        }

        public void Minimize()
        {
            Miniaturize(null);
        }

        public void Unminimize()
        {
            Deminiaturize(null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                webview.Dispose();
            }

            base.Dispose(disposing);
        }

        private class CocoaWindowDelegate : NSWindowDelegate
        {
            private readonly CocoaWindow cocoaWindow;

            public CocoaWindowDelegate(CocoaWindow cocoaWindow)
            {
                this.cocoaWindow = cocoaWindow;
            }

            public override bool WindowShouldClose(NSObject sender)
            {
                var args = new CancelableEventArgs();
                cocoaWindow.Closing?.Invoke(cocoaWindow, args);
                return !args.Cancel;
            }

            public override void WillClose(NSNotification notification)
            {
                cocoaWindow.webview.TitleChanged -= cocoaWindow.Webview_TitleChanged;
                cocoaWindow.Closed?.Invoke(cocoaWindow, EventArgs.Empty);
            }
        }

        private void Webview_TitleChanged(object? sender, string title)
        {
            if (UseBrowserTitle)
            {
                Application.Invoke(() => Title = title ?? string.Empty);
            }
        }

        private static NSWindowStyle GetWantedStyleMask(NSWindowStyle styleMask, WindowBorderStyle borderStyle, bool canResize)
        {
            bool isFullscreen = styleMask.HasFlag(NSWindowStyle.FullScreenWindow);
            NSWindowStyle style = NSWindowStyle.Closable | NSWindowStyle.Miniaturizable;
            style |= borderStyle switch
            {
                WindowBorderStyle.Default => NSWindowStyle.Titled,
                WindowBorderStyle.None => NSWindowStyle.Borderless,
                _ => throw new ArgumentException($"Invalid border style value of {borderStyle}", nameof(BorderStyle)),
            };
            if (canResize) { style |= NSWindowStyle.Resizable; }
            if (isFullscreen) { style |= NSWindowStyle.FullScreenWindow; }

            return style;
        }
    }
}

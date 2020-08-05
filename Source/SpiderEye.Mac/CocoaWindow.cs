using System;
using System.Runtime.InteropServices;
using SpiderEye.Bridge;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaWindow : IWindow
    {
        public event CancelableEventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler Shown;

        public string Title
        {
            get { return NSString.GetString(ObjC.Call(Handle, "title")); }
            set { ObjC.Call(Handle, "setTitle:", NSString.Create(value ?? string.Empty)); }
        }

        public Size Size
        {
            get
            {
                var frame = Marshal.PtrToStructure<CGRect>(ObjC.Call(Handle, "frame"));
                return new Size((int)frame.Size.Width, (int)frame.Size.Height);
            }
            set
            {
                ObjC.Call(Handle, "setContentSize:", new CGSize(value.Width, value.Height));
            }
        }

        public Size MinSize
        {
            get
            {
                var size = Marshal.PtrToStructure<CGSize>(ObjC.Call(Handle, "contentMinSize"));
                return new Size(size.Width, size.Height);
            }
            set
            {
                ObjC.Call(Handle, "setContentMinSize:", new CGSize(value.Width, value.Height));
            }
        }

        public Size MaxSize
        {
            get
            {
                var size = Marshal.PtrToStructure<CGSize>(ObjC.Call(Handle, "contentMaxSize"));
                return new Size(size.Width, size.Height);
            }
            set
            {
                if (value == Size.Zero) { value = new Size(float.MaxValue, float.MaxValue); }

                ObjC.Call(Handle, "setContentMaxSize:", new CGSize(value.Width, value.Height));
            }
        }

        public bool CanResize
        {
            get { return canResizeField; }
            set
            {
                canResizeField = value;
                StyleMask = GetWantedStyleMask();
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
                string title = Title;
                StyleMask = GetWantedStyleMask();
                Title = title;
            }
        }

        public string BackgroundColor
        {
            get { return backgroundColorField; }
            set
            {
                backgroundColorField = value;
                IntPtr bgColor = NSColor.FromHex(value);
                ObjC.Call(Handle, "setBackgroundColor:", bgColor);
                webview.UpdateBackgroundColor(bgColor);
            }
        }

        public bool UseBrowserTitle
        {
            get { return webview.UseBrowserTitle; }
            set { webview.UseBrowserTitle = value; }
        }

        // is ignored because there are no window icons on macOS
        public AppIcon Icon { get; set; }

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

        private NSWindowStyleMask StyleMask
        {
            get { return (NSWindowStyleMask)ObjC.Call(Handle, "styleMask"); }
            set { ObjC.Call(Handle, "setStyleMask:", new IntPtr((int)value)); }
        }

        public readonly IntPtr Handle;

        private static readonly NativeClassDefinition WindowDelegateDefinition;

        private readonly NativeClassInstance windowDelegate;
        private readonly CocoaWebview webview;

        private bool canResizeField;
        private WindowBorderStyle borderStyleField;
        private string backgroundColorField;

        static CocoaWindow()
        {
            WindowDelegateDefinition = CreateWindowDelegate();
        }

        public CocoaWindow(WindowConfiguration config, WebviewBridge bridge)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (bridge == null) { throw new ArgumentNullException(nameof(bridge)); }

            Handle = AppKit.Call("NSWindow", "alloc");

            canResizeField = config.CanResize;
            var style = GetWantedStyleMask();
            ObjC.SendMessage(
                Handle,
                ObjC.RegisterName("initWithContentRect:styleMask:backing:defer:"),
                new CGRect(0, 0, config.Size.Width, config.Size.Height),
                new UIntPtr((uint)style),
                new UIntPtr(2),
                false);

            webview = new CocoaWebview(bridge);
            ObjC.Call(Handle, "setContentView:", webview.Handle);

            webview.TitleChanged += Webview_TitleChanged;

            windowDelegate = WindowDelegateDefinition.CreateInstance(this);
            ObjC.Call(Handle, "setDelegate:", windowDelegate.Handle);
        }

        public void Show()
        {
            ObjC.Call(Handle, "center");
            ObjC.Call(Handle, "makeKeyAndOrderFront:", IntPtr.Zero);

            MacApplication.SynchronizationContext.Post(s => Shown?.Invoke(this, EventArgs.Empty), null);
        }

        public void Close()
        {
            ObjC.Call(Handle, "close", IntPtr.Zero);
        }

        public void EnterFullscreen()
        {
            if (!StyleMask.HasFlag(NSWindowStyleMask.FullScreen))
            {
                ObjC.Call(Handle, "toggleFullScreen:", Handle);
            }
        }

        public void ExitFullscreen()
        {
            if (StyleMask.HasFlag(NSWindowStyleMask.FullScreen))
            {
                ObjC.Call(Handle, "toggleFullScreen:", Handle);
            }
        }

        public void Maximize()
        {
            if (ObjC.Call(Handle, "isZoomed") == IntPtr.Zero) { ObjC.Call(Handle, "zoom:", Handle); }
        }

        public void Unmaximize()
        {
            if (ObjC.Call(Handle, "isZoomed") != IntPtr.Zero) { ObjC.Call(Handle, "zoom:", Handle); }
        }

        public void Minimize()
        {
            ObjC.Call(Handle, "miniaturize:", IntPtr.Zero);
        }

        public void Unminimize()
        {
            ObjC.Call(Handle, "deminiaturize:", IntPtr.Zero);
        }

        public void Dispose()
        {
            // window will be released automatically
            webview.Dispose();
            windowDelegate.Dispose();
        }

        private static NativeClassDefinition CreateWindowDelegate()
        {
            var definition = NativeClassDefinition.FromObject(
                "SpiderEyeWindowDelegate",
                AppKit.GetProtocol("NSWindowDelegate"));

            definition.AddMethod<WindowShouldCloseDelegate>(
                "windowShouldClose:",
                "c@:@",
                (self, op, window) =>
                {
                    var instance = definition.GetParent<CocoaWindow>(self);
                    var args = new CancelableEventArgs();
                    instance.Closing?.Invoke(instance, args);

                    return args.Cancel ? (byte)0 : (byte)1;
                });

            definition.AddMethod<NotificationDelegate>(
                "windowWillClose:",
                "v@:@",
                (self, op, notification) =>
                {
                    var instance = definition.GetParent<CocoaWindow>(self);
                    instance.webview.TitleChanged -= instance.Webview_TitleChanged;
                    instance.Closed?.Invoke(instance, EventArgs.Empty);
                });

            definition.FinishDeclaration();

            return definition;
        }

        private NSWindowStyleMask GetWantedStyleMask()
        {
            bool isFullscreen = StyleMask.HasFlag(NSWindowStyleMask.FullScreen);
            NSWindowStyleMask style = NSWindowStyleMask.Closable | NSWindowStyleMask.Miniaturizable;
            switch (borderStyleField)
            {
                case WindowBorderStyle.Default:
                    style |= NSWindowStyleMask.Titled;
                    break;

                case WindowBorderStyle.None:
                    style |= NSWindowStyleMask.Borderless;
                    break;

                default:
                    throw new ArgumentException($"Invalid border style value of {borderStyleField}", nameof(BorderStyle));
            }

            if (canResizeField) { style |= NSWindowStyleMask.Resizable; }
            if (isFullscreen) { style |= NSWindowStyleMask.FullScreen; }

            return style;
        }

        private void Webview_TitleChanged(object sender, string title)
        {
            if (UseBrowserTitle)
            {
                Application.Invoke(() => Title = title ?? string.Empty);
            }
        }
    }
}

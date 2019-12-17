using System;
using System.Runtime.InteropServices;
using System.Threading;
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
                var style = GetStyleMask(value);
                ObjC.Call(Handle, "setStyleMask:", style);
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

        public readonly IntPtr Handle;

        private static int count = 0;

        private readonly CocoaWebview webview;

        private readonly NotificationDelegate windowShownDelegate;
        private readonly WindowShouldCloseDelegate windowShouldCloseDelegate;
        private readonly NotificationDelegate windowWillCloseDelegate;

        private bool canResizeField;
        private string backgroundColorField;

        public CocoaWindow(WindowConfiguration config, WebviewBridge bridge)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            if (bridge == null) { throw new ArgumentNullException(nameof(bridge)); }

            Interlocked.Increment(ref count);

            // need to keep the delegates around or they will get garbage collected
            windowShownDelegate = WindowShownCallback;
            windowShouldCloseDelegate = WindowShouldCloseCallback;
            windowWillCloseDelegate = WindowWillCloseCallback;

            Handle = AppKit.Call("NSWindow", "alloc");

            var style = GetStyleMask(config.CanResize);
            ObjC.SendMessage(
                Handle,
                ObjC.RegisterName("initWithContentRect:styleMask:backing:defer:"),
                new CGRect(0, 0, config.Size.Width, config.Size.Height),
                style,
                new UIntPtr(2),
                false);

            webview = new CocoaWebview(bridge);
            ObjC.Call(Handle, "setContentView:", webview.Handle);

            webview.TitleChanged += Webview_TitleChanged;

            SetWindowDelegate(Handle);
        }

        public void Show()
        {
            ObjC.Call(Handle, "center");
            ObjC.Call(Handle, "makeKeyAndOrderFront:", IntPtr.Zero);
        }

        public void Close()
        {
            ObjC.Call(Handle, "close", IntPtr.Zero);
        }

        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    // TODO: restore window state when maximized
                    ObjC.Call(Handle, "deminiaturize", IntPtr.Zero);
                    break;

                case WindowState.Maximized:
                    // TODO: maximize window
                    // [window setFrame:[[window screen] frame] display YES]
                    break;

                case WindowState.Minimized:
                    ObjC.Call(Handle, "miniaturize", IntPtr.Zero);
                    break;

                default:
                    throw new ArgumentException($"Invalid window state of \"{state}\"", nameof(state));
            }
        }

        public void Dispose()
        {
            // window will be released automatically
            webview.Dispose();
        }

        private UIntPtr GetStyleMask(bool canResize)
        {
            var style = NSWindowStyleMask.Titled | NSWindowStyleMask.Closable | NSWindowStyleMask.Miniaturizable;
            if (canResize) { style |= NSWindowStyleMask.Resizable; }

            return new UIntPtr((uint)style);
        }

        private void SetWindowDelegate(IntPtr window)
        {
            IntPtr windowDelegateClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "WindowDelegate" + count, IntPtr.Zero);
            ObjC.AddProtocol(windowDelegateClass, ObjC.GetProtocol("NSWindowDelegate"));

            ObjC.AddMethod(
                windowDelegateClass,
                ObjC.RegisterName("windowDidExpose:"),
                windowShownDelegate,
                "v@:@");

            ObjC.AddMethod(
                windowDelegateClass,
                ObjC.RegisterName("windowShouldClose:"),
                windowShouldCloseDelegate,
                "c@:@");

            ObjC.AddMethod(
                windowDelegateClass,
                ObjC.RegisterName("windowWillClose:"),
                windowWillCloseDelegate,
                "v@:@");

            ObjC.RegisterClassPair(windowDelegateClass);

            IntPtr windowDelegate = ObjC.Call(windowDelegateClass, "new");
            ObjC.Call(window, "setDelegate:", windowDelegate);
        }

        private void WindowShownCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            Shown?.Invoke(this, EventArgs.Empty);
        }

        private byte WindowShouldCloseCallback(IntPtr self, IntPtr op, IntPtr window)
        {
            var args = new CancelableEventArgs();
            Closing?.Invoke(this, args);

            return args.Cancel ? (byte)0 : (byte)1;
        }

        private void WindowWillCloseCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            webview.TitleChanged -= Webview_TitleChanged;

            Closed?.Invoke(this, EventArgs.Empty);
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

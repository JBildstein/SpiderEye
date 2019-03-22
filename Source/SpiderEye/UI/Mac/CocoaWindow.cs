using System;
using System.Threading;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaWindow : IWindow
    {
        public event PageLoadEventHandler PageLoaded
        {
            add { webview.PageLoaded += value; }
            remove { webview.PageLoaded -= value; }
        }

        public event CancelableEventHandler Closing;
        public event EventHandler Closed;

        public string Title
        {
            get { return NSString.GetString(ObjC.Call(Handle, "title")); }
            set { ObjC.Call(Handle, "setTitle:", NSString.Create(value ?? string.Empty)); }
        }

        public IWebviewBridge Bridge
        {
            get { return bridge; }
        }

        public readonly IntPtr Handle;

        private static int count = 0;

        private readonly AppConfiguration config;
        private readonly CocoaWebview webview;
        private readonly WebviewBridge bridge;

        public CocoaWindow(AppConfiguration config, IWindowFactory windowFactory)
        {
            if (windowFactory == null) { throw new ArgumentNullException(nameof(windowFactory)); }

            this.config = config ?? throw new ArgumentNullException(nameof(config));

            Interlocked.Increment(ref count);
            Handle = AppKit.Call("NSWindow", "alloc");

            var style = NSWindowStyleMask.Titled | NSWindowStyleMask.Closable | NSWindowStyleMask.Miniaturizable;
            if (config.Window.CanResize) { style |= NSWindowStyleMask.Resizable; }

            ObjC.SendMessage(
                Handle,
                ObjC.RegisterName("initWithContentRect:styleMask:backing:defer:"),
                new CGRect(0, 0, config.Window.Width, config.Window.Height),
                (int)style,
                2,
                0);

            Title = config.Window.Title;

            IntPtr bgColor = NSColor.FromHex(config.Window.BackgroundColor);
            ObjC.Call(Handle, "setBackgroundColor:", bgColor);

            var contentProvider = new EmbeddedFileProvider(config.ContentAssembly, config.ContentFolder);
            bridge = new WebviewBridge();
            webview = new CocoaWebview(config, contentProvider, bridge);
            ObjC.Call(Handle, "setContentView:", webview.Handle);

            if (config.EnableScriptInterface) { bridge.Init(this, webview, windowFactory); }

            if (config.Window.UseBrowserTitle)
            {
                webview.TitleChanged += Webview_TitleChanged;
                bridge.TitleChanged += Webview_TitleChanged;
            }

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

        public void LoadUrl(string url)
        {
            webview.NavigateToFile(url);
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

        public void SetIcon(WindowIcon icon)
        {
            // windows on macOS don't have icons
        }

        public void Dispose()
        {
            // will be released automatically
        }

        private void Webview_TitleChanged(object sender, string title)
        {
            Title = title ?? config.Window.Title;
        }

        private void SetWindowDelegate(IntPtr window)
        {
            IntPtr windowDelegateClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "WindowDelegate" + count, IntPtr.Zero);
            ObjC.AddProtocol(windowDelegateClass, ObjC.GetProtocol("NSWindowDelegate"));

            ObjC.AddMethod(
                windowDelegateClass,
                ObjC.RegisterName("windowShouldClose:"),
                (WindowShouldCloseDelegate)WindowShouldCloseCallback,
                "c@:@");

            ObjC.AddMethod(
                windowDelegateClass,
                ObjC.RegisterName("windowWillClose:"),
                (NotificationDelegate)WindowWillCloseCallback,
                "v@:@");

            ObjC.RegisterClassPair(windowDelegateClass);

            IntPtr windowDelegate = ObjC.Call(windowDelegateClass, "new");
            ObjC.Call(window, "setDelegate:", windowDelegate);
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
            bridge.TitleChanged -= Webview_TitleChanged;

            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}

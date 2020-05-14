using System;
using SpiderEye.Bridge;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkWindow : IWindow
    {
        public event CancelableEventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler Shown;

        public string Title
        {
            get { return GLibString.FromPointer(Gtk.Window.GetTitle(Handle)); }
            set
            {
                using (GLibString str = value)
                {
                    Gtk.Window.SetTitle(Handle, str);
                }
            }
        }

        public Size Size
        {
            get
            {
                Gtk.Window.GetSize(Handle, out int width, out int height);
                return new Size(width, height);
            }
            set
            {
                Gtk.Window.Resize(Handle, (int)value.Width, (int)value.Height);
            }
        }

        public Size MinSize
        {
            get { return minSizeField; }
            set
            {
                minSizeField = value;
                SetWindowRestrictions(minSizeField, maxSizeField);
            }
        }

        public Size MaxSize
        {
            get { return maxSizeField; }
            set
            {
                maxSizeField = value;
                SetWindowRestrictions(minSizeField, maxSizeField);
            }
        }

        public bool CanResize
        {
            get { return Gtk.Window.GetResizable(Handle); }
            set { Gtk.Window.SetResizable(Handle, value); }
        }

        public string BackgroundColor
        {
            get { return backgroundColorField; }
            set
            {
                backgroundColorField = value;
                SetBackgroundColor(value);
                webview.UpdateBackgroundColor(value);
            }
        }

        public bool UseBrowserTitle
        {
            get { return webview.UseBrowserTitle; }
            set { webview.UseBrowserTitle = value; }
        }

        public AppIcon Icon
        {
            get { return iconField; }
            set
            {
                iconField = value;
                SetIcon(value);
            }
        }

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

        public Menu Menu { get; set; } // TODO implement this

        public IWebview Webview
        {
            get { return webview; }
        }

        object IWindow.NativeOptions => this;

        public readonly IntPtr Handle;

        private readonly GtkWebview webview;
        private readonly WebviewBridge bridge;

        private readonly WidgetCallbackDelegate showDelegate;
        private readonly DeleteCallbackDelegate deleteDelegate;
        private readonly WidgetCallbackDelegate destroyDelegate;

        private Size minSizeField;
        private Size maxSizeField;
        private string backgroundColorField;
        private AppIcon iconField;

        public GtkWindow(WebviewBridge bridge)
        {
            this.bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));

            webview = new GtkWebview(bridge);
            Handle = Gtk.Window.Create(GtkWindowType.Toplevel);

            IntPtr scroller = Gtk.Window.CreateScrolled(IntPtr.Zero, IntPtr.Zero);
            Gtk.Widget.ContainerAdd(Handle, scroller);
            Gtk.Widget.ContainerAdd(scroller, webview.Handle);

            // need to keep the delegates around or they will get garbage collected
            showDelegate = ShowCallback;
            deleteDelegate = DeleteCallback;
            destroyDelegate = DestroyCallback;

            GLib.ConnectSignal(Handle, "show", destroyDelegate, IntPtr.Zero);
            GLib.ConnectSignal(Handle, "delete-event", deleteDelegate, IntPtr.Zero);
            GLib.ConnectSignal(Handle, "destroy", destroyDelegate, IntPtr.Zero);

            webview.CloseRequested += Webview_CloseRequested;
            webview.TitleChanged += Webview_TitleChanged;
        }

        public void Show()
        {
            Gtk.Widget.ShowAll(Handle);
            Gtk.Window.Present(Handle);
        }

        public void ShowModal(IWindow modalWindow)
        {
            // TODO implement this
            throw new NotImplementedException();
        }

        public void Close()
        {
            Gtk.Window.Close(Handle);
        }

        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    Gtk.Window.Unmaximize(Handle);
                    Gtk.Window.Unminimize(Handle);
                    break;

                case WindowState.Maximized:
                    Gtk.Window.Maximize(Handle);
                    break;

                case WindowState.Minimized:
                    Gtk.Window.Minimize(Handle);
                    break;

                default:
                    throw new ArgumentException($"Invalid window state of \"{state}\"", nameof(state));
            }
        }

        public void Dispose()
        {
            webview.Dispose();
            Gtk.Widget.Destroy(Handle);
        }

        private void SetWindowRestrictions(Size min, Size max)
        {
            var geometry = new GdkGeometry(min, max);
            GdkWindowHints hints = 0;
            if (min != Size.Zero) { hints |= GdkWindowHints.MinSize; }
            if (max != Size.Zero) { hints |= GdkWindowHints.MaxSize; }

            Gtk.Window.SetGeometryHints(Handle, IntPtr.Zero, ref geometry, hints);
        }

        private unsafe void SetIcon(AppIcon icon)
        {
            if (icon == null || icon.Icons.Length == 0)
            {
                Gtk.Window.SetIcon(Handle, IntPtr.Zero);
            }
            else
            {
                IntPtr iconList = IntPtr.Zero;
                var icons = new IntPtr[icon.Icons.Length];

                try
                {
                    for (int i = 0; i < icons.Length; i++)
                    {
                        IntPtr iconStream = IntPtr.Zero;
                        try
                        {
                            byte[] data = icon.GetIconData(icon.Icons[i]);
                            fixed (byte* iconDataPtr = data)
                            {
                                iconStream = GLib.CreateStreamFromData((IntPtr)iconDataPtr, data.Length, IntPtr.Zero);
                                icons[i] = Gdk.Pixbuf.NewFromStream(iconStream, IntPtr.Zero, IntPtr.Zero);
                                iconList = GLib.ListPrepend(iconList, icons[i]);
                            }
                        }
                        finally { if (iconStream != IntPtr.Zero) { GLib.UnrefObject(iconStream); } }
                    }

                    Gtk.Window.SetIconList(Handle, iconList);
                }
                finally
                {
                    if (iconList != IntPtr.Zero) { GLib.FreeList(iconList); }
                    foreach (var item in icons)
                    {
                        if (item != IntPtr.Zero) { GLib.UnrefObject(item); }
                    }
                }
            }
        }

        private void ShowCallback(IntPtr widget, IntPtr userdata)
        {
            Shown?.Invoke(this, EventArgs.Empty);
        }

        private bool DeleteCallback(IntPtr widget, IntPtr eventData, IntPtr userdata)
        {
            var args = new CancelableEventArgs();
            Closing?.Invoke(this, args);

            return args.Cancel;
        }

        private void DestroyCallback(IntPtr widget, IntPtr userdata)
        {
            webview.TitleChanged -= Webview_TitleChanged;
            bridge.TitleChanged -= Webview_TitleChanged;

            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Webview_TitleChanged(object sender, string title)
        {
            if (UseBrowserTitle)
            {
                Application.Invoke(() => Title = title ?? string.Empty);
            }
        }

        private void Webview_CloseRequested(object sender, EventArgs e)
        {
            Close();
        }

        private void SetBackgroundColor(string color)
        {
            IntPtr provider = IntPtr.Zero;

            try
            {
                provider = Gtk.Css.Create();

                using (GLibString css = $"* {{background-color:{color}}}")
                {
                    Gtk.Css.LoadData(provider, css, new IntPtr(-1), IntPtr.Zero);
                }

                IntPtr context = Gtk.StyleContext.Get(Handle);
                Gtk.StyleContext.AddProvider(context, provider, GtkStyleProviderPriority.Application);
            }
            finally { if (provider != IntPtr.Zero) { GLib.UnrefObject(provider); } }
        }
    }
}

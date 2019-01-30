using System;
using SpiderEye.Bridge;
using SpiderEye.Configuration;
using SpiderEye.Content;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkWindow : IWindow
    {
        public event EventHandler PageLoaded
        {
            add { webview.PageLoaded += value; }
            remove { webview.PageLoaded -= value; }
        }

        public event EventHandler Closing;
        public event EventHandler Closed;

        public IWebview Webview
        {
            get { return webview; }
        }

        public string Title
        {
            get { return GLibString.FromPointer(Gtk.Window.GetTitle(window)); }
            set
            {
                using (GLibString str = value)
                {
                    Gtk.Window.SetTitle(window, str);
                }
            }
        }

        public IWebviewBridge Bridge
        {
            get { return bridge; }
        }

        private readonly IntPtr window;
        private readonly AppConfiguration config;
        private readonly GtkWebview webview;
        private readonly WebviewBridge bridge;

        public GtkWindow(AppConfiguration config, IWindowFactory windowFactory)
        {
            if (windowFactory == null) { throw new ArgumentNullException(nameof(windowFactory)); }

            this.config = config ?? throw new ArgumentNullException(nameof(config));

            var contentProvider = new EmbeddedFileProvider(config.ContentAssembly, config.ContentFolder);
            bridge = new WebviewBridge();
            webview = new GtkWebview(config, contentProvider, bridge);
            window = Gtk.Window.Create(GtkWindowType.Toplevel);

            Title = config.Window.Title;
            Gtk.Window.SetResizable(window, config.Window.CanResize);
            Gtk.Window.SetDefaultSize(window, config.Window.Width, config.Window.Height);

            string backgroundColor = config.Window.BackgroundColor;
            if (string.IsNullOrWhiteSpace(backgroundColor)) { backgroundColor = "#FFFFFF"; }
            SetBackgroundColor(backgroundColor);

            IntPtr scroller = Gtk.Window.CreateScrolled(IntPtr.Zero, IntPtr.Zero);
            Gtk.Widget.ContainerAdd(window, scroller);
            Gtk.Widget.ContainerAdd(scroller, webview.Handle);

            GLib.ConnectSignal(window, "delete-event", (DeleteCallbackDelegate)DeleteCallback, IntPtr.Zero);
            GLib.ConnectSignal(window, "destroy", (DestroyCallbackDelegate)DestroyCallback, IntPtr.Zero);

            webview.CloseRequested += Webview_CloseRequested;

            if (config.EnableScriptInterface) { bridge.Init(this, webview, windowFactory); }

            if (config.Window.UseBrowserTitle)
            {
                webview.TitleChanged += Webview_TitleChanged;
                bridge.TitleChanged += Webview_TitleChanged;
            }
        }

        public void Show()
        {
            Gtk.Widget.ShowAll(window);
            Gtk.Window.Present(window);
        }

        public void Close()
        {
            Gtk.Window.Close(window);
        }

        public void SetWindowState(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    Gtk.Window.Unmaximize(window);
                    Gtk.Window.Unminimize(window);
                    break;

                case WindowState.Maximized:
                    Gtk.Window.Maximize(window);
                    break;

                case WindowState.Minimized:
                    Gtk.Window.Minimize(window);
                    break;

                default:
                    throw new ArgumentException($"Invalid window state of \"{state}\"", nameof(state));
            }
        }

        public void LoadUrl(string url)
        {
            webview.NavigateToFile(url);
        }

        public void Dispose()
        {
            webview.Dispose();
            Gtk.Widget.Destroy(window);
        }

        private bool DeleteCallback(IntPtr widget, IntPtr eventData, IntPtr userdata)
        {
            Closing?.Invoke(this, EventArgs.Empty);
            return false;
        }

        private void DestroyCallback(IntPtr widget, IntPtr userdata)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Webview_TitleChanged(object sender, string title)
        {
            Title = title ?? config.Window.Title;
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

                IntPtr context = Gtk.StyleContext.Get(window);
                Gtk.StyleContext.AddProvider(context, provider, GtkStyleProviderPriority.Application);
            }
            finally { if (provider != IntPtr.Zero) { GLib.UnrefObject(provider); } }
        }
    }
}

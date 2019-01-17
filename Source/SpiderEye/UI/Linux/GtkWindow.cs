using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkWindow : IWindow
    {
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

        public int Width
        {
            get
            {
                Gtk.Window.GetSize(window, out int width, out int height);
                return width;
            }
        }

        public int Height
        {
            get
            {
                Gtk.Window.GetSize(window, out int width, out int height);
                return height;
            }
        }

        public string BackgroundColor { get; set; }

        public bool CanResize
        {
            get { return Gtk.Window.GetResizable(window); }
            set { Gtk.Window.SetResizable(window, value); }
        }

        private readonly IntPtr window;
        private readonly WindowConfiguration config;
        private readonly GtkWebview webview;

        public GtkWindow(WindowConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            webview = new GtkWebview(config.EnableScriptInterface);
            window = Gtk.Window.Create(GtkWindowType.Toplevel);

            Title = config.Title;
            CanResize = config.CanResize;
            Gtk.Window.SetDefaultSize(window, config.Width, config.Height);

            BackgroundColor = config.BackgroundColor;
            if (string.IsNullOrWhiteSpace(BackgroundColor)) { BackgroundColor = "#FFFFFF"; }
            SetBackgroundColor(BackgroundColor);

            IntPtr scroller = Gtk.Window.CreateScrolled(IntPtr.Zero, IntPtr.Zero);
            Gtk.Widget.ContainerAdd(window, scroller);
            Gtk.Widget.ContainerAdd(scroller, webview.Handle);

            using (GLibString name = "destroy")
            {
                GLib.ConnectSignal(window, name, (DestroyCallbackDelegate)DestroyCallback, IntPtr.Zero);
            }

            webview.CloseRequested += Webview_CloseRequested;

            if (config.UseBrowserTitle)
            {
                webview.TitleChanged += Webview_TitleChanged;
                webview.ScriptHandler.TitleChanged += Webview_TitleChanged;
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

        public void Resize(int width, int height)
        {
            Gtk.Window.Resize(window, width, height);
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
            webview.LoadUrl(url);
        }

        public void Dispose()
        {
            webview.Dispose();
            Gtk.Widget.Destroy(window);
        }

        private void DestroyCallback(IntPtr widget, IntPtr arg)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Webview_TitleChanged(object sender, string title)
        {
            Title = title ?? config.Title;
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

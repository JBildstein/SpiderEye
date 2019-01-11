using System;

namespace SpiderEye.Linux
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

        private readonly IntPtr window;
        private readonly GtkWebview webview;

        public GtkWindow(AppConfiguration config, GtkWebview webview)
        {
            this.webview = webview ?? throw new ArgumentNullException(nameof(webview));

            window = Gtk.Window.Create(GtkWindowType.Toplevel);

            Title = config.Title;
            Gtk.Window.SetDefaultSize(window, config.Width, config.Height);
            Gtk.Window.SetResizable(window, config.CanResize);

            IntPtr scroller = Gtk.Window.CreateScrolled(IntPtr.Zero, IntPtr.Zero);
            Gtk.Window.ContainerAdd(window, scroller);
            Gtk.Window.ContainerAdd(scroller, webview.Handle);

            using (GLibString name = "destroy")
            {
                GLib.ConnectSignal(window, name, (DestroyCallbackDelegate)DestroyCallback, IntPtr.Zero);
            }

            webview.CloseRequested += Webview_CloseRequested;
            webview.TitleChanged += Webview_TitleChanged;
            webview.ScriptHandler.TitleChanged += Webview_TitleChanged;
        }

        public void Show()
        {
            Gtk.Window.ShowAll(window);
            Gtk.Window.Present(window);
        }

        public void Close()
        {
            Gtk.Window.Close(window);
        }

        public void Destroy()
        {
            Gtk.Window.Destroy(window);
        }

        private void DestroyCallback(IntPtr widget, IntPtr arg)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Webview_TitleChanged(object sender, string title)
        {
            Title = title;
        }

        private void Webview_CloseRequested(object sender, EventArgs e)
        {
            Gtk.Window.Close(window);
        }
    }
}

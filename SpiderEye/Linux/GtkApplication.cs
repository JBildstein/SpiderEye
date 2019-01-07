using System;

namespace SpiderEye.Linux
{
    internal class GtkApplication : IApplication
    {
        public IWindow MainWindow
        {
            get { return window; }
        }

        private readonly AppConfiguration config;
        private readonly GtkWindow window;
        private volatile bool keepRunning = true;

        public GtkApplication(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            Init();

            var webview = new GtkWebview(config);
            window = new GtkWindow(config, webview);

            window.Closed += Window_Closed;
        }

        public void Run()
        {
            window.Webview.LoadUrl(config.Url);
            window.Show();

            while (keepRunning)
            {
                Gtk.MainIteration();
            }
        }

        private static void Init()
        {
            var argv = new IntPtr(0);
            int argc = 0;
            if (!Gtk.Init(ref argc, ref argv))
            {
                throw new Exception("Could not initialize GTK+");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            keepRunning = false;
        }
    }
}

using System;

namespace SpiderEye.Linux
{
    internal class GtkApplication : ApplicationBase
    {
        public override IWindow MainWindow
        {
            get { return window; }
        }

        public override IWebview Webview
        {
            get { return webview; }
        }

        private readonly GtkWindow window;
        private readonly GtkWebview webview;
        private volatile bool keepRunning = true;

        public GtkApplication(AppConfiguration config)
            : base(config)
        {
            Init();

            webview = new GtkWebview(config);
            window = new GtkWindow(config, webview);

            window.Closed += Window_Closed;
        }

        public override void Run()
        {
            base.Run();

            while (keepRunning)
            {
                Gtk.MainIteration();
            }

            window.Destroy();
        }

        public override void Exit()
        {
            keepRunning = false;
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
            Exit();
        }
    }
}

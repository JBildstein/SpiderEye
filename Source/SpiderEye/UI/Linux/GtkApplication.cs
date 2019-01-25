using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkApplication : ApplicationBase
    {
        public override IWindow MainWindow
        {
            get { return window; }
        }

        private readonly GtkWindow window;

        public GtkApplication(AppConfiguration config)
            : base(config)
        {
            Init();

            window = new GtkWindow(config);
            window.Closed += Window_Closed;
        }

        public override void Exit()
        {
            Gtk.Quit();
        }

        protected override void RunMainLoop()
        {
            MainWindow.LoadUrl(config.StartPageUrl);

            Gtk.Main();
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

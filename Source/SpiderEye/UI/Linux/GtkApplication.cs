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

        public override IWindowFactory WindowFactory
        {
            get { return factory; }
        }

        private readonly GtkWindow window;
        private readonly GtkWindowFactory factory;

        public GtkApplication(AppConfiguration config)
            : base(config)
        {
            Init();

            factory = new GtkWindowFactory(config);
            window = new GtkWindow(config);
            window.Closed += Window_Closed;
        }

        public override void Exit()
        {
            Gtk.Quit();
        }

        protected override void RunMainLoop()
        {
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

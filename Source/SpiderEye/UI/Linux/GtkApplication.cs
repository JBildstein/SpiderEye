using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkApplication : ApplicationBase
    {
        public override IWindow MainWindow { get; }
        public override IWindowFactory Factory { get; }

        public GtkApplication(AppConfiguration config)
            : base(config)
        {
            Init();

            Factory = new GtkWindowFactory(config);
            var window = new GtkWindow(config, Factory);
            window.Closed += Window_Closed;
            MainWindow = window;
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

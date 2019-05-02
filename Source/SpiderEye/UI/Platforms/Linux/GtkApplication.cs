using System;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkApplication : IApplication
    {
        public bool ExitWithLastWindow { get; set; }
        public IUiFactory Factory { get; }

        public GtkApplication()
        {
            Init();

            ExitWithLastWindow = true;
            Factory = new GtkUiFactory();
            GtkWindow.LastWindowClosed += (s, e) => { if (ExitWithLastWindow) { Exit(); } };
        }

        public void Run()
        {
            Gtk.Main();
        }

        public void Exit()
        {
            Gtk.Quit();
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

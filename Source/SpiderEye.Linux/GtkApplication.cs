using System;
using System.Threading;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkApplication : IApplication
    {
        public IUiFactory Factory { get; }

        public SynchronizationContext SynchronizationContext { get; }

        private bool hasExited = false;

        public GtkApplication()
        {
            Init();

            Factory = new GtkUiFactory();
            SynchronizationContext = new GtkSynchronizationContext();

            Application.OpenWindows.AllWindowsClosed += Application_AllWindowsClosed;
        }

        public void Run()
        {
            Gtk.Main();
        }

        public void Exit()
        {
            if (!hasExited)
            {
                hasExited = true;
                Gtk.Quit();
            }
        }

        private void Init()
        {
            var argv = IntPtr.Zero;
            int argc = 0;
            if (!Gtk.Init(ref argc, ref argv))
            {
                throw new InvalidOperationException("Could not initialize GTK+");
            }
        }

        private void Application_AllWindowsClosed(object sender, EventArgs e)
        {
            if (Application.ExitWithLastWindow) { Exit(); }
        }
    }
}

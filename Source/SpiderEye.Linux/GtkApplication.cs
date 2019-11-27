using System;
using System.Threading;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkApplication : IApplication
    {
        public IUiFactory Factory { get; }

        private bool hasExited = false;

        public GtkApplication()
        {
            Init();

            Factory = new GtkUiFactory();
            GtkWindow.LastWindowClosed += GtkWindow_LastWindowClosed;
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

        public void Invoke(Action action)
        {
            using (var mre = new ManualResetEventSlim(false))
            {
                // if on main thread, callback is executed immediately
                // and mre is set before calling Wait().
                // Otherwise we block the calling thread until the action is executed.
                GLib.ContextInvoke(
                    IntPtr.Zero,
                    data =>
                    {
                        try { action(); }
                        finally { mre.Set(); }

                        return false;
                    },
                    IntPtr.Zero);

                mre.Wait();
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

        private void GtkWindow_LastWindowClosed(object sender, EventArgs e)
        {
            if (Application.ExitWithLastWindow) { Exit(); }
        }
    }
}

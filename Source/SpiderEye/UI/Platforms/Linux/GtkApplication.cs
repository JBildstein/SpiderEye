using System;
using System.Threading;
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

        public IMenu CreateAppMenu()
        {
            return null;
        }

        public IMenu CreateDefaultAppMenu()
        {
            return null;
        }

        public void Run()
        {
            Gtk.Main();
        }

        public void Exit()
        {
            Gtk.Quit();
        }

        public void Invoke(Action action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

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

        private static void Init()
        {
            var argv = new IntPtr(0);
            int argc = 0;
            if (!Gtk.Init(ref argc, ref argv))
            {
                throw new InvalidOperationException("Could not initialize GTK+");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Exit();
        }
    }
}

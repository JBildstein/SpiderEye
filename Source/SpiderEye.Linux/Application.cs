using System;
using System.Threading;
using SpiderEye.UI.Linux;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye
{
    /// <content>
    /// Linux specific implementations.
    /// </content>
    public static partial class Application
    {
        static Application()
        {
            OS = GetOS();
            CheckOs(OperatingSystem.Linux);

            Init();

            Factory = new GtkUiFactory();
            GtkWindow.LastWindowClosed += (s, e) => { if (ExitWithLastWindow) { Exit(); } };
        }

        static partial void RunImpl()
        {
            Gtk.Main();
        }

        static partial void ExitImpl()
        {
            Gtk.Quit();
        }

        static partial void InvokeImpl(Action action)
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

        private static void Init()
        {
            var argv = IntPtr.Zero;
            int argc = 0;
            if (!Gtk.Init(ref argc, ref argv))
            {
                throw new InvalidOperationException("Could not initialize GTK+");
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            ExitImpl();
        }
    }
}

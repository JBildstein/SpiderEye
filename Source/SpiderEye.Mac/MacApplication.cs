using System;
using System.Threading;
using SpiderEye.Mac.Native;
using SpiderEye.Tools;

namespace SpiderEye.Mac
{
    /// <summary>
    /// Provides macOS specific application methods.
    /// </summary>
    public static class MacApplication
    {
        internal static Menu WindowMenu
        {
            get => windowMenu;
            set
            {
                windowMenu = value;
                SetMenu();
            }
        }

        internal static IntPtr Handle
        {
            get { return app.Handle; }
        }

        internal static SynchronizationContext SynchronizationContext
        {
            get { return app.SynchronizationContext; }
        }

        private static CocoaApplication app;
        private static Menu windowMenu;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Init()
        {
            app = new CocoaApplication();
            Application.Register(app, OperatingSystem.MacOS);
        }

        private static void SetMenu()
        {
            var menu = windowMenu ?? new Menu();
            var nativeMenu = NativeCast.To<CocoaMenu>(menu.NativeMenu);
            ObjC.Call(Handle, "setMainMenu:", nativeMenu?.Handle ?? IntPtr.Zero);
        }
    }
}

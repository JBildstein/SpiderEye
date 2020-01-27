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
        /// <summary>
        /// Gets or sets the application menu.
        /// </summary>
        public static Menu AppMenu
        {
            get { return appMenu; }
            set
            {
                appMenu = value;
                SetAppMenu(value);
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
        private static Menu appMenu;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Init()
        {
            app = new CocoaApplication();
            Application.Register(app, OperatingSystem.MacOS);
            AppMenu = CreateDefaultMenu();
        }

        private static void SetAppMenu(Menu menu)
        {
            var nativeMenu = NativeCast.To<CocoaMenu>(menu.NativeMenu);
            ObjC.Call(Handle, "setMainMenu:", nativeMenu?.Handle ?? IntPtr.Zero);
        }

        private static Menu CreateDefaultMenu()
        {
            var menu = new Menu();
            var appMenu = menu.MenuItems.AddLabelItem(string.Empty);
            appMenu.MenuItems.AddAboutItem();
            appMenu.MenuItems.AddSeparatorItem();
            appMenu.MenuItems.AddHideItem();
            appMenu.MenuItems.AddHideOthersItem();
            appMenu.MenuItems.AddShowAllItem();
            appMenu.MenuItems.AddSeparatorItem();
            appMenu.MenuItems.AddQuitItem();

            return menu;
        }
    }
}

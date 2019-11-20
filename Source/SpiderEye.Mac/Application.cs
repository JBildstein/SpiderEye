using System;
using SpiderEye.UI;
using SpiderEye.UI.Mac;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Menu;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye
{
    /// <content>
    /// Mac specific implementations.
    /// </content>
    public static partial class Application
    {
        private static readonly ShouldTerminateDelegate ShouldTerminateDelegateRef;
        private static readonly NotificationDelegate AppFinishedLaunchingDelegateRef;

        private static readonly IntPtr AppHandle;
        private static IMenu appMenu; // needed to prevent garbage collection

        static Application()
        {
            OS = GetOS();
            CheckOs(OperatingSystem.MacOS);

            Factory = new CocoaUiFactory();

            // need to keep the delegates around or they will get garbage collected
            ShouldTerminateDelegateRef = ShouldTerminateCallback;
            AppFinishedLaunchingDelegateRef = AppFinishedLaunching;

            AppHandle = GetApp();
            ObjC.Call(AppHandle, "setActivationPolicy:", IntPtr.Zero);

            IntPtr appDelegateClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "AppDelegate", IntPtr.Zero);
            ObjC.AddProtocol(appDelegateClass, ObjC.GetProtocol("NSApplicationDelegate"));

            ObjC.AddMethod(
                appDelegateClass,
                ObjC.RegisterName("applicationShouldTerminateAfterLastWindowClosed:"),
                ShouldTerminateDelegateRef,
                "c@:@");

            ObjC.AddMethod(
                appDelegateClass,
                ObjC.RegisterName("applicationDidFinishLaunching:"),
                AppFinishedLaunchingDelegateRef,
                "v@:@");

            ObjC.RegisterClassPair(appDelegateClass);

            IntPtr appDelegate = ObjC.Call(appDelegateClass, "new");
            ObjC.Call(AppHandle, "setDelegate:", appDelegate);

            CreateDefaultAppMenu();
        }

        /// <summary>
        /// Creates and adds an empty app menu.
        /// </summary>
        /// <returns>The app menu.</returns>
        public static IMenu CreateAppMenu()
        {
            var menu = new CocoaMenu();
            ObjC.Call(AppHandle, "setMainMenu:", menu.Handle);

            return appMenu = menu;
        }

        /// <summary>
        /// Creates and adds an app menu with default values.
        /// </summary>
        /// <returns>The app menu.</returns>
        public static IMenu CreateDefaultAppMenu()
        {
            // TODO: use app name in menu items
            var menu = CreateAppMenu();
            var appMenu = menu.AddLabelMenuItem(string.Empty);

            var about = appMenu.AddLabelMenuItem("About");
            about.Click += (s, e) => ObjC.Call(AppHandle, "orderFrontStandardAboutPanel:", AppHandle);

            appMenu.AddSeparatorMenuItem();

            var hide = appMenu.AddLabelMenuItem("Hide");
            hide.SetShortcut(ModifierKey.Super, Key.H);
            hide.Click += (s, e) => ObjC.Call(AppHandle, "hide:", AppHandle);

            var hideOthers = appMenu.AddLabelMenuItem("Hide Others");
            hideOthers.SetShortcut(ModifierKey.Super | ModifierKey.Alt, Key.H);
            hideOthers.Click += (s, e) => ObjC.Call(AppHandle, "hideOtherApplications:", AppHandle);

            var showAll = appMenu.AddLabelMenuItem("Show All");
            showAll.Click += (s, e) => ObjC.Call(AppHandle, "unhideAllApplications:", AppHandle);

            appMenu.AddSeparatorMenuItem();

            var quit = appMenu.AddLabelMenuItem("Quit");
            quit.SetShortcut(ModifierKey.Super, Key.Q);
            quit.Click += (s, e) => Exit();

            return Application.appMenu = menu;
        }

        internal static IntPtr GetApp()
        {
            return AppKit.Call("NSApplication", "sharedApplication");
        }

        static partial void RunImpl()
        {
            ObjC.Call(AppHandle, "run");
        }

        static partial void ExitImpl()
        {
            ObjC.Call(AppHandle, "terminate:", AppHandle);
        }

        static partial void InvokeImpl(Action action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            bool isMainThread = Foundation.Call("NSThread", "isMainThread") != IntPtr.Zero;
            if (isMainThread) { action(); }
            else
            {
                Dispatch.SyncFunction(Dispatch.MainQueue, IntPtr.Zero, ctx => action());
            }
        }

        private static byte ShouldTerminateCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            return (byte)(ExitWithLastWindow ? 1 : 0);
        }

        private static void AppFinishedLaunching(IntPtr self, IntPtr op, IntPtr notification)
        {
            ObjC.Call(AppHandle, "activateIgnoringOtherApps:", 1);
        }
    }
}

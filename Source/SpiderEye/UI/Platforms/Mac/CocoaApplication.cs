using System;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Menu;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaApplication : IApplication
    {
        public bool ExitWithLastWindow { get; set; }
        public IUiFactory Factory { get; }

        private readonly IntPtr application;

        public CocoaApplication()
        {
            Factory = new CocoaUiFactory();
            ExitWithLastWindow = true;

            application = GetApp();
            ObjC.Call(application, "setActivationPolicy:", IntPtr.Zero);

            IntPtr appDelegateClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "AppDelegate", IntPtr.Zero);
            ObjC.AddProtocol(appDelegateClass, ObjC.GetProtocol("NSApplicationDelegate"));

            ObjC.AddMethod(
                appDelegateClass,
                ObjC.RegisterName("applicationShouldTerminateAfterLastWindowClosed:"),
                (ShouldTerminateDelegate)ShouldTerminateCallback,
                "c@:@");

            ObjC.AddMethod(
                appDelegateClass,
                ObjC.RegisterName("applicationDidFinishLaunching:"),
                (NotificationDelegate)AppFinishedLaunching,
                "v@:@");

            ObjC.RegisterClassPair(appDelegateClass);

            IntPtr appDelegate = ObjC.Call(appDelegateClass, "new");
            ObjC.Call(application, "setDelegate:", appDelegate);
        }

        public static IntPtr GetApp()
        {
            return AppKit.Call("NSApplication", "sharedApplication");
        }

        public IMenu CreateAppMenu()
        {
            var menu = new CocoaMenu();
            ObjC.Call(application, "setMainMenu:", menu.Handle);

            return menu;
        }

        public IMenu CreateDefaultAppMenu()
        {
            // TODO: use app name in menu items

            var menu = CreateAppMenu();
            var appMenu = menu.AddLabelMenuItem("");

            var about = appMenu.AddLabelMenuItem("About");
            about.Click += (s, e) => ObjC.Call(application, "orderFrontStandardAboutPanel:", application);

            appMenu.AddSeparatorMenuItem();

            var hide = appMenu.AddLabelMenuItem("Hide");
            hide.SetShortcut(ModifierKey.Super, Key.H);
            hide.Click += (s, e) => ObjC.Call(application, "hide:", application);

            var hideOthers = appMenu.AddLabelMenuItem("Hide Others");
            hideOthers.SetShortcut(ModifierKey.Super | ModifierKey.Alt, Key.H);
            hideOthers.Click += (s, e) => ObjC.Call(application, "hideOtherApplications:", application);

            var showAll = appMenu.AddLabelMenuItem("Show All");
            showAll.Click += (s, e) => ObjC.Call(application, "unhideAllApplications:", application);

            appMenu.AddSeparatorMenuItem();

            var quit = appMenu.AddLabelMenuItem("Quit");
            quit.SetShortcut(ModifierKey.Super, Key.Q);
            quit.Click += (s, e) => Exit();

            return menu;
        }

        public void Run()
        {
            ObjC.Call(application, "run");
        }

        public void Exit()
        {
            ObjC.Call(application, "terminate:", application);
        }

        private byte ShouldTerminateCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            return (byte)(ExitWithLastWindow ? 1 : 0);
        }

        private void AppFinishedLaunching(IntPtr self, IntPtr op, IntPtr notification)
        {
            ObjC.Call(application, "activateIgnoringOtherApps:", 1);
        }
    }
}

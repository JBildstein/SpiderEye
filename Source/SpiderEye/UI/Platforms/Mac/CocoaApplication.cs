using System;
using SpiderEye.UI.Mac.Interop;
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

            ObjC.RegisterClassPair(appDelegateClass);

            IntPtr appDelegate = ObjC.Call(appDelegateClass, "new");
            ObjC.Call(application, "setDelegate:", appDelegate);
        }

        public static IntPtr GetApp()
        {
            return AppKit.Call("NSApplication", "sharedApplication");
        }

        public void Run()
        {
            ObjC.Call(application, "activateIgnoringOtherApps:", 1);
            ObjC.Call(application, "run");
        }

        public void Exit()
        {
            ObjC.Call(application, "stop");
        }

        private byte ShouldTerminateCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            return (byte)(ExitWithLastWindow ? 1 : 0);
        }
    }
}

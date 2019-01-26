using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaApplication : ApplicationBase
    {
        public override IWindow MainWindow { get; }
        public override IWindowFactory Factory { get; }

        private readonly IntPtr application;

        public CocoaApplication(AppConfiguration config)
            : base(config)
        {
            Factory = new CocoaWindowFactory(config);

            application = AppKit.Call("NSApplication", "sharedApplication");
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

            MainWindow = new CocoaWindow(config, Factory);
        }

        public override void Exit()
        {
            ObjC.Call(application, "stop");
        }

        protected override void RunMainLoop()
        {
            ObjC.Call(application, "activateIgnoringOtherApps:", 1);
            ObjC.Call(application, "run");
        }

        private byte ShouldTerminateCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            return 1;
        }
    }
}

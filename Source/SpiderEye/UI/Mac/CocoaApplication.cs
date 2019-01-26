using System;
using SpiderEye.Configuration;
using SpiderEye.UI.Mac.Interop;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac
{
    internal class CocoaApplication : ApplicationBase
    {
        public override IWindow MainWindow
        {
            get { return window; }
        }

        public override IWindowFactory WindowFactory
        {
            get { return factory; }
        }

        private readonly IntPtr application;
        private readonly CocoaWindow window;
        private readonly CocoaWindowFactory factory;

        public CocoaApplication(AppConfiguration config)
            : base(config)
        {
            factory = new CocoaWindowFactory(config);

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

            window = new CocoaWindow(config);
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

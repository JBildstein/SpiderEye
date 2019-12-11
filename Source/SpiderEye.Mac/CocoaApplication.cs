using System;
using System.Threading;
using SpiderEye.Mac.Interop;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac
{
    internal class CocoaApplication : IApplication
    {
        public IUiFactory Factory { get; }

        public SynchronizationContext SynchronizationContext { get; }

        public IntPtr Handle { get; }

        private readonly ShouldTerminateDelegate shouldTerminateDelegateRef;
        private readonly NotificationDelegate appFinishedLaunchingDelegateRef;

        public CocoaApplication()
        {
            Factory = new CocoaUiFactory();
            SynchronizationContext = new CocoaSynchronizationContext();

            // need to keep the delegates around or they will get garbage collected
            shouldTerminateDelegateRef = ShouldTerminateCallback;
            appFinishedLaunchingDelegateRef = AppFinishedLaunching;

            Handle = AppKit.Call("NSApplication", "sharedApplication");
            ObjC.Call(Handle, "setActivationPolicy:", IntPtr.Zero);

            IntPtr appDelegateClass = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "AppDelegate", IntPtr.Zero);
            ObjC.AddProtocol(appDelegateClass, ObjC.GetProtocol("NSApplicationDelegate"));

            ObjC.AddMethod(
                appDelegateClass,
                ObjC.RegisterName("applicationShouldTerminateAfterLastWindowClosed:"),
                shouldTerminateDelegateRef,
                "c@:@");

            ObjC.AddMethod(
                appDelegateClass,
                ObjC.RegisterName("applicationDidFinishLaunching:"),
                appFinishedLaunchingDelegateRef,
                "v@:@");

            ObjC.RegisterClassPair(appDelegateClass);

            IntPtr appDelegate = ObjC.Call(appDelegateClass, "new");
            ObjC.Call(Handle, "setDelegate:", appDelegate);
        }

        public void Run()
        {
            ObjC.Call(Handle, "run");
        }

        public void Exit()
        {
            ObjC.Call(Handle, "terminate:", Handle);
        }

        private byte ShouldTerminateCallback(IntPtr self, IntPtr op, IntPtr notification)
        {
            return (byte)(Application.ExitWithLastWindow ? 1 : 0);
        }

        private void AppFinishedLaunching(IntPtr self, IntPtr op, IntPtr notification)
        {
            ObjC.Call(Handle, "activateIgnoringOtherApps:", true);
        }
    }
}

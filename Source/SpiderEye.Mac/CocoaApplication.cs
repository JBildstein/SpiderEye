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

        private static readonly NativeClassDefinition AppDelegateDefinition;
        private readonly NativeClassInstance appDelegate;

        static CocoaApplication()
        {
            AppDelegateDefinition = CreateAppDelegate();
        }

        public CocoaApplication()
        {
            Factory = new CocoaUiFactory();
            SynchronizationContext = new CocoaSynchronizationContext();

            Handle = AppKit.Call("NSApplication", "sharedApplication");
            appDelegate = AppDelegateDefinition.CreateInstance(this);

            ObjC.Call(Handle, "setActivationPolicy:", IntPtr.Zero);
            ObjC.Call(Handle, "setDelegate:", appDelegate.Handle);
        }

        public void Run()
        {
            ObjC.Call(Handle, "run");
        }

        public void Exit()
        {
            ObjC.Call(Handle, "terminate:", Handle);
            appDelegate.Dispose();
        }

        private static NativeClassDefinition CreateAppDelegate()
        {
            var definition = NativeClassDefinition.FromClass(
                "SpiderEyeAppDelegate",
                AppKit.GetClass("NSResponder"),
                // note: NSApplicationDelegate is not available at runtime and returns null
                AppKit.GetProtocol("NSApplicationDelegate"),
                AppKit.GetProtocol("NSTouchBarProvider"));

            definition.AddMethod<ShouldTerminateDelegate>(
                "applicationShouldTerminateAfterLastWindowClosed:",
                "c@:@",
                (self, op, notification) => (byte)(Application.ExitWithLastWindow ? 1 : 0));

            definition.AddMethod<NotificationDelegate>(
                "applicationDidFinishLaunching:",
                "v@:@",
                (self, op, notification) =>
                {
                    var instance = definition.GetParent<CocoaApplication>(self);
                    ObjC.Call(instance.Handle, "activateIgnoringOtherApps:", true);
                });

            definition.FinishDeclaration();

            return definition;
        }
    }
}

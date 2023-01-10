using System;
using System.Threading;
using AppKit;
using Foundation;

namespace SpiderEye.Mac
{
    internal class CocoaApplication : IApplication
    {
        public IUiFactory Factory { get; }

        public SynchronizationContext SynchronizationContext { get; }

        public CocoaApplication()
        {
            NSApplication.Init();

            Factory = new CocoaUiFactory();
            SynchronizationContext = SynchronizationContext.Current!;

            NSApplication.SharedApplication.Delegate = new CocoaAppDelegate();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;
        }

        public void Run()
        {
            NSApplication.SharedApplication.Run();
        }

        public void Exit()
        {
            NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication);
        }

        private class CocoaAppDelegate : NSApplicationDelegate
        {
            public override void DidFinishLaunching(NSNotification notification)
            {
                NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            }

            public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
            {
                return Application.ExitWithLastWindow;
            }
        }
    }
}

using System;

namespace SpiderEye.Mac.Interop
{
    internal delegate byte ShouldTerminateDelegate(IntPtr self, IntPtr op, IntPtr notification);
    internal delegate void ScriptCallbackDelegate(IntPtr self, IntPtr op, IntPtr notification, IntPtr msg);
    internal delegate void ScriptEvalCallbackDelegate(IntPtr self, IntPtr result, IntPtr error);
    internal delegate void LoadFinishedDelegate(IntPtr self, IntPtr op, IntPtr view, IntPtr navigation);
    internal delegate void LoadFailedDelegate(IntPtr self, IntPtr op, IntPtr view, IntPtr navigation, IntPtr error);
    internal delegate void SchemeHandlerDelegate(IntPtr self, IntPtr op, IntPtr view, IntPtr schemeTask);
    internal delegate void ObserveValueDelegate(IntPtr self, IntPtr op, IntPtr keyPath, IntPtr obj, IntPtr change, IntPtr context);
    internal delegate byte WindowShouldCloseDelegate(IntPtr self, IntPtr op, IntPtr window);
    internal delegate void NotificationDelegate(IntPtr self, IntPtr op, IntPtr notification);
    internal delegate void MenuCallbackDelegate(IntPtr self, IntPtr op, IntPtr menu);
    internal delegate void DispatchDelegate(IntPtr context);
}

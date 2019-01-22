using System;

namespace SpiderEye.UI.Mac.Interop
{
    internal delegate byte ShouldTerminateDelegate(IntPtr self, IntPtr op, IntPtr notification);
    internal delegate void ScriptCallbackDelegate(IntPtr self, IntPtr op, IntPtr notification, IntPtr msg);
    internal delegate void ScriptEvalCallbackDelegate(IntPtr self, IntPtr result, IntPtr error);
    internal delegate void LoadFinishedDelegate(IntPtr self, IntPtr op, IntPtr view, IntPtr navigation);
    internal delegate void ObserveValueDelegate(IntPtr self, IntPtr op, IntPtr keyPath, IntPtr obj, IntPtr change, IntPtr context);
}

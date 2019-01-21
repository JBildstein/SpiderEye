using System;

namespace SpiderEye.UI.Mac.Interop
{
    internal delegate void DispatchCallbackDelegate(IntPtr context);
    internal delegate byte ShouldTerminateDelegate(IntPtr self, IntPtr op, IntPtr notification);
    internal delegate void ScriptCallbackDelegate(IntPtr self, IntPtr op, IntPtr notification, IntPtr msg);
}

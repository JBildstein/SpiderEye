using System;

namespace SpiderEye.UI.Linux.Interop
{
    internal delegate void GAsyncReadyDelegate(IntPtr gobject, IntPtr result, IntPtr userdata);
    internal delegate bool GSourceDelegate(IntPtr userdata);
    internal delegate void WebKitUriSchemeRequestDelegate(IntPtr request, IntPtr userdata);
    internal delegate void MenuActivateDelegate(IntPtr menu, IntPtr userdata);
    internal delegate void DestroyCallbackDelegate(IntPtr widget, IntPtr userdata);
    internal delegate bool DeleteCallbackDelegate(IntPtr widget, IntPtr eventData, IntPtr userdata);
    internal delegate void WebviewDelegate(IntPtr webview, IntPtr userdata);
    internal delegate void ScriptDelegate(IntPtr manager, IntPtr jsResult, IntPtr userdata);
    internal delegate void PageLoadDelegate(IntPtr webview, WebKitLoadEvent type, IntPtr userdata);
    internal delegate bool PageLoadFailedDelegate(IntPtr webview, WebKitLoadEvent type, IntPtr failingUrl, IntPtr error, IntPtr userdata);
    internal delegate bool ContextMenuRequestDelegate(IntPtr webview, IntPtr default_menu, IntPtr hit_test_result, bool triggered_with_keyboard, IntPtr userdata);
}

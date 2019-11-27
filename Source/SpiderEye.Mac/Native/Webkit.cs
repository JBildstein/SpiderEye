using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Mac.Native
{
    internal static class WebKit
    {
        private const string WebKitFramework = "/System/Library/Frameworks/WebKit.framework/WebKit";

        [DllImport(WebKitFramework, EntryPoint = "objc_getClass", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetClass(string name);

        public static IntPtr Call(string id, string sel)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel));
        }
    }
}

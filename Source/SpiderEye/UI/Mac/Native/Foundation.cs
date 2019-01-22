using System;
using System.Runtime.InteropServices;

namespace SpiderEye.UI.Mac.Native
{
    internal static class Foundation
    {
        private const string FoundationFramework = "/System/Library/Frameworks/Foundation.framework/Foundation";

        [DllImport(FoundationFramework, EntryPoint = "objc_getClass", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetClass(string name);

        public static IntPtr Call(string id, string sel)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel));
        }

        public static IntPtr Call(IntPtr id, string sel)
        {
            return ObjC.SendMessage(id, ObjC.RegisterName(sel));
        }

        public static IntPtr Call(string id, string sel, IntPtr a)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a);
        }

        public static IntPtr Call(string id, string sel, int a)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a);
        }
    }
}

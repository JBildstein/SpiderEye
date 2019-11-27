using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Mac.Native
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

        public static IntPtr Call(string id, string sel, IntPtr a)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a);
        }

        public static IntPtr Call(string id, string sel, IntPtr a, IntPtr b)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a, b);
        }

        public static IntPtr Call(string id, string sel, IntPtr a, IntPtr b, IntPtr c)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a, b, c);
        }

        public static IntPtr Call(string id, string sel, IntPtr[] a, IntPtr b)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a, b);
        }

        public static IntPtr Call(string id, string sel, bool a)
        {
            return ObjC.SendMessage(GetClass(id), ObjC.RegisterName(sel), a);
        }
    }
}

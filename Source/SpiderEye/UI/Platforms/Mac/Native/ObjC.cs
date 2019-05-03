using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Mac.Interop;

namespace SpiderEye.UI.Mac.Native
{
    internal static class ObjC
    {
        private const string ObjCLib = "/usr/lib/libobjc.dylib";

        [DllImport(ObjCLib, EntryPoint = "objc_getClass", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetClass(string name);

        [DllImport(ObjCLib, EntryPoint = "objc_allocateClassPair", CharSet = CharSet.Ansi)]
        public static extern IntPtr AllocateClassPair(IntPtr superclass, string name, IntPtr extraBytes);

        [DllImport(ObjCLib, EntryPoint = "objc_registerClassPair")]
        public static extern void RegisterClassPair(IntPtr cls);

        [DllImport(ObjCLib, EntryPoint = "class_addProtocol")]
        public static extern bool AddProtocol(IntPtr cls, IntPtr protocol);

        [DllImport(ObjCLib, EntryPoint = "objc_getProtocol", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProtocol(string name);

        [DllImport(ObjCLib, EntryPoint = "class_addMethod", CharSet = CharSet.Ansi)]
        public static extern bool AddMethod(IntPtr cls, IntPtr name, Delegate imp, string types);

        [DllImport(ObjCLib, EntryPoint = "sel_registerName", CharSet = CharSet.Ansi)]
        public static extern IntPtr RegisterName(string name);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, UIntPtr a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, IntPtr b);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, IntPtr b, IntPtr c);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, IntPtr b, IntPtr c, IntPtr d);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr a, UIntPtr b);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, IntPtr[] a, IntPtr b);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, int value);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, double value);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, [MarshalAs(UnmanagedType.Bool)] byte a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, double a, double b, double c, double d);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGRect rect, IntPtr a);

        [DllImport(ObjCLib, EntryPoint = "objc_msgSend")]
        public static extern IntPtr SendMessage(IntPtr self, IntPtr op, CGRect rect, int a, int b, int c);


        public static IntPtr Call(IntPtr id, string sel)
        {
            return SendMessage(id, RegisterName(sel));
        }

        public static IntPtr Call(IntPtr id, string sel, IntPtr a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, UIntPtr a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, int a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, double a)
        {
            return SendMessage(id, RegisterName(sel), a);
        }

        public static IntPtr Call(IntPtr id, string sel, bool a)
        {
            return SendMessage(id, RegisterName(sel), a ? 1 : 0);
        }

        public static IntPtr Call(IntPtr id, string sel, IntPtr a, IntPtr b)
        {
            return SendMessage(id, RegisterName(sel), a, b);
        }

        public static IntPtr Call(IntPtr id, string sel, IntPtr a, IntPtr b, IntPtr c)
        {
            return SendMessage(id, RegisterName(sel), a, b, c);
        }

        public static IntPtr Call(IntPtr id, string sel, IntPtr a, IntPtr b, IntPtr c, IntPtr d)
        {
            return SendMessage(id, RegisterName(sel), a, b, c, d);
        }

        public static IntPtr Call(IntPtr id, string sel, CGRect rect, IntPtr a)
        {
            return SendMessage(id, RegisterName(sel), rect, a);
        }
    }
}

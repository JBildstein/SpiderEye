using System;
using System.Text;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac.Interop
{
    internal static class NSString
    {
        public static unsafe IntPtr Create(string? value)
        {
            if (value == null) { return IntPtr.Zero; }

            fixed (char* ptr = value)
            {
                return ObjC.SendMessage(
                    ObjC.GetClass("NSString"),
                    ObjC.RegisterName("stringWithCharacters:length:"),
                    (IntPtr)ptr,
                    new UIntPtr((uint)value.Length));
            }
        }

        public static string? GetString(IntPtr handle)
        {
            if (handle == IntPtr.Zero) { return null; }

            IntPtr utf8 = ObjC.Call(handle, "UTF8String");
            return Utf8PointerToString(utf8);
        }

        public static unsafe string? Utf8PointerToString(IntPtr utf8)
        {
            if (utf8 == IntPtr.Zero) { return null; }

            int count = 0;
            byte* ptr = (byte*)utf8;
            while (*ptr != 0)
            {
                count++;
                ptr++;
            }

            return Encoding.UTF8.GetString((byte*)utf8, count);
        }
    }
}

using System;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Interop
{
    internal static class NSString
    {
        public static unsafe void Use(string value, Action<IntPtr> callback)
        {
            if (value == null) { return; }

            fixed (char* ptr = value)
            {
                IntPtr nsString = ObjC.SendMessage(
                    ObjC.GetClass("NSString"),
                    ObjC.RegisterName("stringWithCharacters:length:"),
                    (IntPtr)ptr,
                    new UIntPtr((uint)value.Length));

                callback(nsString);
            }
        }
    }
}

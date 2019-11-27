using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Linux.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GError
    {
        public readonly uint Domain;
        public readonly int Code;
        public readonly IntPtr Message;

        public GError(uint domain, int code, IntPtr message)
        {
            Domain = domain;
            Code = code;
            Message = message;
        }
    }
}

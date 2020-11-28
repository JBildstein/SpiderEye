using System;
using System.Runtime.InteropServices;
using System.Text;
using SpiderEye.Linux.Native;

namespace SpiderEye.Linux.Interop
{
    internal struct GLibString : IDisposable
    {
        public readonly IntPtr Pointer;

        public GLibString(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public GLibString(string? value)
        {
            if (value == null)
            {
                Pointer = IntPtr.Zero;
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Pointer = GLib.Malloc(new UIntPtr((ulong)bytes.Length + 1));
            Marshal.Copy(bytes, 0, Pointer, bytes.Length);
            Marshal.WriteByte(Pointer, bytes.Length, 0);
        }

        public static string? FromPointer(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero) { return null; }

            int length = GetLength(pointer);
            byte[] bytes = new byte[length];
            Marshal.Copy(pointer, bytes, 0, length);

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static implicit operator GLibString(string? value)
        {
            return new GLibString(value);
        }

        public static implicit operator IntPtr(GLibString value)
        {
            return value.Pointer;
        }

        public override string? ToString()
        {
            return FromPointer(Pointer);
        }

        public void Dispose()
        {
            GLib.Free(Pointer);
        }

        private static unsafe int GetLength(IntPtr pointer)
        {
            int length = 0;
            byte* ptr = (byte*)pointer;
            while (*ptr != 0)
            {
                length++;
                ptr++;
            }

            return length;
        }
    }
}

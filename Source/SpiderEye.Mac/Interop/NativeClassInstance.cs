using System;
using System.Runtime.InteropServices;

namespace SpiderEye.Mac.Interop
{
    internal sealed class NativeClassInstance : IDisposable
    {
        public IntPtr Handle { get; }

        private readonly GCHandle parentHandle;

        internal NativeClassInstance(IntPtr instance, GCHandle parentHandle)
        {
            Handle = instance;
            this.parentHandle = parentHandle;
        }

        ~NativeClassInstance()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            parentHandle.Free();
        }
    }
}

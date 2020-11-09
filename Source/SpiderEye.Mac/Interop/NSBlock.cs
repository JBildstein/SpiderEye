using System;
using System.Runtime.InteropServices;
using SpiderEye.Mac.Native;

namespace SpiderEye.Mac.Interop
{
    internal sealed class NSBlock : IDisposable
    {
        public readonly IntPtr Handle;
        private GCHandle CallbackHandle;

        public unsafe NSBlock(Delegate callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            CallbackHandle = GCHandle.Alloc(callback);
            var blp = (BlockLiteral*)Marshal.AllocHGlobal(sizeof(BlockLiteral));
            var bdp = (BlockDescriptor*)Marshal.AllocHGlobal(sizeof(BlockDescriptor));

            blp->Isa = ObjC.GetClass("__NSStackBlock");
            blp->Flags = 0;
            blp->Reserved = 0;
            blp->Invoke = Marshal.GetFunctionPointerForDelegate(callback);
            blp->Descriptor = bdp;

            bdp->Reserved = IntPtr.Zero;
            bdp->Size = new IntPtr(sizeof(BlockLiteral));
            bdp->CopyHelper = IntPtr.Zero;
            bdp->DisposeHelper = IntPtr.Zero;

            Handle = (IntPtr)blp;
        }

        public unsafe void Dispose()
        {
            var blp = (BlockLiteral*)Handle;

            CallbackHandle.Free();
            Marshal.FreeHGlobal((IntPtr)blp->Descriptor);
            Marshal.FreeHGlobal(Handle);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct BlockLiteral
        {
            public IntPtr Isa;
            public BlockFlags Flags;
            public int Reserved;
            public IntPtr Invoke;
            public BlockDescriptor* Descriptor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BlockDescriptor
        {
            public IntPtr Reserved;
            public IntPtr Size;
            public IntPtr CopyHelper;
            public IntPtr DisposeHelper;
        }

        [Flags]
        public enum BlockFlags : int
        {
            RefcountMask = 0xFFFF,
            NeedsFree = 1 << 24,
            HasCopyDispose = 1 << 25,
            HasCxxObj = 1 << 26,
            IsGC = 1 << 27,
            IsGlobal = 1 << 28,
            HasDescriptor = 1 << 29,
            HasSignature = 1 << 30,
        }
    }
}

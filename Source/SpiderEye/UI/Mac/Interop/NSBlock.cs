using System;
using System.Runtime.InteropServices;
using SpiderEye.UI.Mac.Native;

namespace SpiderEye.UI.Mac.Interop
{
    internal sealed class NSBlock : IDisposable
    {
        public readonly IntPtr Handle;

        private NSBlock(IntPtr handle)
        {
            Handle = handle;
        }

        public static unsafe NSBlock Create<T>(T @delegate)
        {
            var blp = (BlockLiteral*)Marshal.AllocHGlobal(sizeof(BlockLiteral));
            var bdp = (BlockDescriptor*)Marshal.AllocHGlobal(sizeof(BlockDescriptor));

            blp->Isa = ObjC.GetClass("__NSStackBlock");
            blp->Flags = 0;
            blp->Reserved = 0;
            blp->Invoke = Marshal.GetFunctionPointerForDelegate(@delegate);
            blp->Descriptor = bdp;

            bdp->Reserved = IntPtr.Zero;
            bdp->Size = new IntPtr(sizeof(BlockLiteral));
            bdp->CopyHelper = IntPtr.Zero;
            bdp->DisposeHelper = IntPtr.Zero;

            return new NSBlock((IntPtr)blp);
        }

        public unsafe void Dispose()
        {
            var blp = (BlockLiteral*)Handle;

            Marshal.FreeHGlobal((IntPtr)blp->Descriptor);
            Marshal.FreeHGlobal(Handle);
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct BlockLiteral
        {
            public IntPtr Isa;
            public int Flags;
            public int Reserved;
            public IntPtr Invoke;
            public BlockDescriptor* Descriptor;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BlockDescriptor
        {
            public IntPtr Reserved;
            public IntPtr Size;
            public IntPtr CopyHelper;
            public IntPtr DisposeHelper;
        }
    }
}
